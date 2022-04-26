using Dapper;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.IService
{
    public interface IDapperService<DB> where DB : IDbContext
    {
        Task<T> QueryFirstOrDefault<T>(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text) 
            where T : class;
        Task<List<T>> SqlQuery<T>(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text) 
            where T : class;
        Task<SqlMapper.GridReader> QueryMultiple(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text);
        Task Execute(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text);
        Task<T> ExecuteScalar<T>(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text);
        Task<DbDataReader> ExecuteReader(string sql, DynamicParameters parameters = null, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text);
        Task<CommonQueryPageResultModel<T>> SqlQueryByPage<T>(string countSql, string querySql, DynamicParameters parameters, CommonPageModel pageModel, 
            int? commandTimeout = null, CommandType commandType = CommandType.Text) 
            where T : class;
    }
}
