using NetCoreProject.Domain.Model;
using System;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.IService
{
    public interface ITokenCacheService
    {
        Task Add(string key, CommonTokenModel value, TimeSpan expiry);
        Task<bool> Exists(string key);
        Task<CommonTokenModel> Get(string key);
        Task Replace(string key, CommonTokenModel value, TimeSpan expiry);
        Task Remove(string key);
        void Dispose();
    }
}
