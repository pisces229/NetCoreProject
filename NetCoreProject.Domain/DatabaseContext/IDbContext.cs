using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.DatabaseContext
{
    public interface IDbContext
    {
        DatabaseFacade GetDatabase();
        Task<DbConnection> GetDbConnection();
        Task<DbTransaction> GetDbTransaction();
        Task BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
    }
}
