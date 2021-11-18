using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NetCoreProject.Domain.Service
{
    public class DapperService<DB> : IDapperService<DB> where DB : IDbContext
    {
        protected readonly ILogger<DapperService<DB>> _logger;
        private readonly DB _dbContext;
        public DapperService(ILogger<DapperService<DB>> logger,
            DB dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<T> QueryFirstOrDefault<T>(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text)
            where T : class
        {
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ? 
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            return await dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, dbTransaction, dbCommandTimeout, commandType);
        }
        public async Task<List<T>> SqlQuery<T>(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text)
            where T : class
        {
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ?
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            return (await dbConnection.QueryAsync<T>(sql, parameters, dbTransaction, dbCommandTimeout, commandType)).ToList();
        }
        public async Task<SqlMapper.GridReader> QueryMultiple(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ?
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            return await dbConnection.QueryMultipleAsync(sql, parameters, dbTransaction, dbCommandTimeout, commandType);
        }
        public async Task Execute(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ?
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            await dbConnection.ExecuteAsync(sql, parameters, dbTransaction, dbCommandTimeout, commandType);
        }
        public async Task<T> ExecuteScalar<T>(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ?
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            return await dbConnection.ExecuteScalarAsync<T>(sql, parameters, dbTransaction, dbCommandTimeout, commandType);
        }
        public async Task<DbDataReader> ExecuteReader(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ?
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            return await dbConnection.ExecuteReaderAsync(sql, parameters, dbTransaction, dbCommandTimeout, commandType);
        }
        public async Task<CommonQueryPageResultModel<T>> SqlQueryByPage<T>(string countSql, string querySql, DynamicParameters parameters, CommonPageModel pageModel, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text) 
            where T : class
        {
            var result = new CommonQueryPageResultModel<T>()
            {
                Page = pageModel,
                Data = new List<T>()
            };
            var dbConnection = await _dbContext.GetDbConnection();
            var dbTransaction = await _dbContext.GetDbTransaction();
            var dbCommandTimeout = commandTimeout == null ?
                _dbContext.GetDatabase().GetCommandTimeout().Value : commandTimeout.Value;
            pageModel.TotalCount = await dbConnection.ExecuteScalarAsync<int>(countSql, parameters, dbTransaction, dbCommandTimeout, commandType);
            if (pageModel.TotalCount > 0 && pageModel.PageNo > 0 && pageModel.PageSize > 0)
            {
                using var dbDataReader = await dbConnection.ExecuteReaderAsync(querySql, parameters, dbTransaction, dbCommandTimeout, commandType);
                if (dbDataReader.HasRows)
                {
                    var rowParserFunc = dbDataReader.GetRowParser<T>();
                    var row = 0;
                    while (dbDataReader.Read())
                    {
                        ++row;
                        if (row > pageModel.PageNo * pageModel.PageSize)
                        {
                            break;
                        }
                        else if (row < ((pageModel.PageNo - 1) * pageModel.PageSize + 1))
                        {
                            continue;
                        }
                        else
                        {
                            result.Data.Add(rowParserFunc(dbDataReader));
                        }
                    }
                }
            }
            return result;
        }
    }
}
