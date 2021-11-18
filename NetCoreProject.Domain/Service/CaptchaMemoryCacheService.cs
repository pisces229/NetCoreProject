using NetCoreProject.Domain.IService;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.Service
{
    public class CaptchaMemoryCacheService : ICaptchaCacheService, IDisposable
    {
        protected readonly IMemoryCache _memoryCache;
        public CaptchaMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task Add(string key, string value, TimeSpan expiry)
        {
            await Task.Run(() => _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiry)));
        }
        public async Task<string> Get(string key)
        {
            return await Task.FromResult(_memoryCache.Get<string>(key));
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
