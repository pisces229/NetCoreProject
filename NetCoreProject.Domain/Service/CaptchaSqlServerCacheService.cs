using NetCoreProject.Domain.IService;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace NetCoreProject.Domain.Service
{
    public class CaptchaSqlServerCacheService : ICaptchaCacheService, IDisposable
    {
        private readonly IDistributedCache _distributedCache;
        public CaptchaSqlServerCacheService(IDistributedCache distributedCache)
        {
            //Microsoft.Extensions.Caching.SqlServer.SqlServerCache
            _distributedCache = distributedCache;
        }
        public async Task Add(string key, string value, TimeSpan expiry)
        {
            await _distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.UtcNow.Add(expiry)
            });
        }
        public async Task<string> Get(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }
        public async Task Remove(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
        public void Dispose()
        {
            //if (_distributedCache != null)
            //{
            //    _distributedCache.Dispose();
            //}
            GC.SuppressFinalize(this);
        }
    }
}
