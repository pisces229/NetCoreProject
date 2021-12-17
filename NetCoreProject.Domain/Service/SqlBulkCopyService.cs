using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.Enum;
using NetCoreProject.Domain.IService;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.Service
{
    public class SqlBulkCopyService<DB> : ISqlBulkCopyService<DB> where DB : IDbContext
    {
        protected readonly ILogger<SqlBulkCopyService<DB>> _logger;
        private readonly DB _dbContext;
        public SqlBulkCopyService(ILogger<SqlBulkCopyService<DB>> logger,
            DB dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task Write<T>(List<T> datas) where T : class
        {
            if (datas.Any())
            {
                var propertyInfoArray = typeof(T).GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var dataTable = new DataTable
                {
                    TableName = typeof(T).Name
                };
                foreach (var propertyInfo in propertyInfoArray)
                {
                    var databaseGenerated = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
                    if (databaseGenerated == null)
                    {
                        dataTable.Columns.Add(propertyInfo.Name);
                    }
                }
                foreach (var data in datas)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var propertyInfo in propertyInfoArray)
                    {
                        var databaseGeneratedAttribute = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
                        if (databaseGeneratedAttribute == null)
                        {
                            var value = propertyInfo.GetValue(data);
                            if (value != null)
                            {
                                var maxLengthAttribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                                if (maxLengthAttribute != null)
                                {
                                    if (value.ToString().Length > maxLengthAttribute.Length)
                                    {
                                        throw new ValidationException(
                                            string.Format("[{0}].[{1}] MAX LENGTH [{2}]"
                                                , typeof(T).Name
                                                , propertyInfo.Name
                                                , maxLengthAttribute.Length));
                                    }
                                }
                                if (typeof(DateTime).Equals(propertyInfo.PropertyType)
                                    ||
                                    typeof(DateTime?).Equals(propertyInfo.PropertyType))
                                {
                                    dataRow[propertyInfo.Name] = ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss.fff");
                                }
                                else
                                {
                                    dataRow[propertyInfo.Name] = value;
                                }
                            }
                            else
                            {
                                if ("UPDATE_USER_ID".Equals(propertyInfo.Name))
                                {
                                    dataRow[propertyInfo.Name] = "default";
                                }
                                else if ("UPDATE_PROG_CD".Equals(propertyInfo.Name))
                                {
                                    dataRow[propertyInfo.Name] = "default";
                                }
                                else if ("UPDATE_DATE_TIME".Equals(propertyInfo.Name))
                                {
                                    dataRow[propertyInfo.Name] = DateTime.Now;
                                }
                                else
                                {
                                    var requiredAttribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
                                    if (requiredAttribute != null)
                                    {
                                        throw new ValidationException(
                                            string.Format("[{0}].[{1}] NOT NULL"
                                                , typeof(T).Name
                                                , propertyInfo.Name));
                                    }
                                }
                            }
                        }
                    }
                    dataTable.Rows.Add(dataRow);
                }
                _logger.LogInformation($"Batch Insert Table [{ dataTable.TableName }] Count [{ dataTable.Rows.Count }]");
                var dbConnection = (SqlConnection)_dbContext.GetDbConnection().Result;
                var dbTransaction = (SqlTransaction)_dbContext.GetDbTransaction().Result;
                //SqlBulkCopyOptions.Default,
                using var sqlBulkCopy = new SqlBulkCopy(dbConnection, SqlBulkCopyOptions.FireTriggers, dbTransaction)
                {
                    BatchSize = 5000
                };
                if (_dbContext.GetDatabase().GetCommandTimeout().HasValue)
                {
                    sqlBulkCopy.BulkCopyTimeout = _dbContext.GetDatabase().GetCommandTimeout().Value;
                }
                sqlBulkCopy.DestinationTableName = "dbo." + dataTable.TableName;
                foreach (DataColumn dc in dataTable.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                await sqlBulkCopy.WriteToServerAsync(dataTable);
            }
        }
    }
}
