using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.Service
{
    public class TokenMemoryCacheService : ITokenCacheService, IDisposable
    {
        protected readonly IMemoryCache _memoryCache;
        public TokenMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task Add(string key, CommonTokenModel value, TimeSpan expiry)
        {
            value.Expiration = DateTime.Now.Add(expiry);
            await Task.Run(() => _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiry)));
        }
        public async Task<bool> Exists(string key)
        { 
            return await Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        public async Task<CommonTokenModel> Get(string key)
        {
            return await Task.FromResult(_memoryCache.Get<CommonTokenModel>(key));
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
            await Task.Run(() => _memoryCache.Remove(key));
        }
        public void Dispose()
        {
            if (_memoryCache != null)
            {
                _memoryCache.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
