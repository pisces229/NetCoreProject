using NetCoreProject.Domain.IService;
using Microsoft.Extensions.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NetCoreProject.Domain.Service
{
    public class DefaultRedisCacheService : IDefaultCacheService, IDisposable
    {
        protected IDatabase _database;
        private ConnectionMultiplexer _connectionMultiplexer;
        public DefaultRedisCacheService(IConfiguration configuration)
        {
            var configurationOptions = ConfigurationOptions.Parse(
                configuration.GetValue<string>("DefaultRedisConnectionString"));
            configurationOptions.CertificateValidation += (request, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            _database = _connectionMultiplexer.GetDatabase();
        }
        public async Task<bool> Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await _database.KeyExistsAsync(key);
        }
        public async Task<bool> Add(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await _database.StringSetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value));
        }
        public async Task<bool> Add(string key, object value, TimeSpan expiryliding, TimeSpan expiryAbsoulte)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await _database.StringSetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), expiryAbsoulte);
        }
        public async Task<bool> Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await _database.StringSetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), expiresIn);
        }
        public async Task<bool> Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await _database.KeyDeleteAsync(key);
        }
        public async Task RemoveAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            await Task.Run(() => keys.ToList().ForEach(async item => await Remove(item)));
        }
        public async Task<T> Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = _database.StringGet(key);
            if (!value.HasValue)
            {
                return await Task.FromResult(default(T));
            }
            return await Task.FromResult(JsonSerializer.Deserialize<T>(value));
        }
        public async Task<object> Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = _database.StringGet(key);
            if (!value.HasValue)
            {
                return await Task.FromResult(default(object));
            }
            return await Task.FromResult(JsonSerializer.Deserialize<object>(value));
        }
        public async Task<IDictionary<string, object>> GetAll(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var result = new Dictionary<string, object>();
            keys.ToList().ForEach(async item => result.Add(item, await Get(item)));
            return await Task.FromResult(result);
        }
        public async Task<bool> Replace(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
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
            if (_connectionMultiplexer != null)
            {
                _connectionMultiplexer.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
