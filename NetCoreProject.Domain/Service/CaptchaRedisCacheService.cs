using NetCoreProject.Domain.IService;
using Microsoft.Extensions.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NetCoreProject.Domain.Service
{
    public class CaptchaRedisCacheService : ICaptchaCacheService, IDisposable
    {
        protected IDatabase _database;
        private ConnectionMultiplexer _connectionMultiplexer;
        public CaptchaRedisCacheService(IConfiguration configuration)
        {
            var configurationOptions = ConfigurationOptions.Parse(
                configuration.GetValue<string>("CaptchaRedisConnectionString"));
            configurationOptions.CertificateValidation += (request, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            _database = _connectionMultiplexer.GetDatabase();
        }
        public async Task Add(string key, string value, TimeSpan expiry)
        {
            await _database.StringSetAsync(key, value, expiry);
        }
        public async Task<string> Get(string key)
        {
            return await _database.StringGetAsync(key);
        }
        public async Task Remove(string key)
        {
            await _database.KeyDeleteAsync(key);
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
