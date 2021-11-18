using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.IService
{
    public interface IDefaultCacheService
    {
        Task<bool> Exists(string key);
        Task<bool> Add(string key, object value);
        Task<bool> Add(string key, object value, TimeSpan expiryliding, TimeSpan expiryAbsoulte);
        Task<bool> Add(string key, object value, TimeSpan expiresIn, bool isSliding = false);
        Task<bool> Remove(string key);
        Task RemoveAll(IEnumerable<string> keys);
        Task<T> Get<T>(string key) where T : class;
        Task<object> Get(string key);
        Task<IDictionary<string, object>> GetAll(IEnumerable<string> keys);
        Task<bool> Replace(string key, object value);
        Task<bool> Replace(string key, object value, TimeSpan expiryliding, TimeSpan expiryAbsoulte);
        Task<bool> Replace(string key, object value, TimeSpan expiresIn, bool isSliding = false);
        void Dispose();
    }
}
