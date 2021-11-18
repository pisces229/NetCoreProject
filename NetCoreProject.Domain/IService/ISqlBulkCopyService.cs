using NetCoreProject.Domain.DatabaseContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.IService
{
    public interface ISqlBulkCopyService<DB> where DB : IDbContext
    {
        Task Write<T>(List<T> datas) where T : class;
    }
}
