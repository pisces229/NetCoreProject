using NetCoreProject.Domain.IService;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.Service
{
    public class DefaultMemoryCacheService : IDefaultCacheService, IDisposable
    {
        protected readonly IMemoryCache _memoryCache;
        public DefaultMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<bool> Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        public async Task<bool> Add(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _memoryCache.Set(key, value);
            return await Exists(key);
        }
        public async Task<bool> Add(string key, object value, TimeSpan expiryliding, TimeSpan expiryAbsoulte)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _memoryCache.Set(key, value,
                new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiryliding)
                .SetAbsoluteExpiration(expiryAbsoulte));
            return await Exists(key);
        }
        public async Task<bool> Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (isSliding)
            {
                _memoryCache.Set(key, value,
                    new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn));
            }
            else
            {
                _memoryCache.Set(key, value,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn));
            }
            return await Exists(key);
        }
        public async Task<bool> Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            _memoryCache.Remove(key);
            return !await Exists(key);
        }
        public async Task RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            await Task.Run(() => keys.ToList().ForEach(item => _memoryCache.Remove(item)));
        }
        public async Task<T> Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await Task.FromResult(_memoryCache.Get<T>(key));
        }
        public async Task<object> Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await Task.FromResult(_memoryCache.Get(key));
        }
        public async Task<IDictionary<string, object>> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var result = new Dictionary<string, object>();
            keys.ToList().ForEach(item => result.Add(item, _memoryCache.Get(item)));
            return await Task.FromResult(result);
        }
        public async Task<bool> Replace(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await Exists(key))
            {
                if (!await Remove(key))
                {
                    return await Task.FromResult(false);
                }
            }
            return await Add(key, value);
        }
        public async Task<bool> Replace(string key, object value, TimeSpan expiryliding, TimeSpan expiryAbsoulte)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await Exists(key))
            {
                if (!await Remove(key))
                {
                    return await Task.FromResult(false);
                }
            }
            return await Add(key, value, expiryliding, expiryAbsoulte);
        }
        public async Task<bool> Replace(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await Exists(key))
            {
                if (!await Remove(key))
                {
                    return await Task.FromResult(false);
                }
            }
            return await Add(key, value, expiresIn, isSliding);
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
