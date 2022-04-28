using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace NetCoreProject.Domain.Service
{
    public class TokenSqlServerCacheService : ITokenCacheService, IDisposable
    {
        private readonly IDistributedCache _distributedCache;
        public TokenSqlServerCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task Add(string key, CommonTokenModel value, TimeSpan expiry)
        {
            value.Expiration = DateTime.Now.Add(expiry);
            await _distributedCache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expiry
            });
        }
        public async Task<bool> Exists(string key)
        {
            var reuslt = _distributedCache.Get(key);
            return await Task.FromResult(reuslt != null);
        }
        public async Task<CommonTokenModel> Get(string key)
        {
            var result = JsonSerializer.Deserialize<CommonTokenModel>(_distributedCache.Get(key));
            return await Task.FromResult(result);
        }
        public async Task Replace(string key, CommonTokenModel value, TimeSpan expiry)
        {
            if (await Exists(key))
            {
                await Remove(key);
                await Add(key, value, expiry);
            }
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
