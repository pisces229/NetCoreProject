using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NetCoreProject.Domain.Service
{
    public class TokenRedisCacheService : ITokenCacheService, IDisposable
    {
        protected IDatabase _database;
        private ConnectionMultiplexer _connectionMultiplexer;
        public TokenRedisCacheService(IConfiguration configuration)
        {
            var configurationOptions = ConfigurationOptions.Parse(
                configuration.GetValue<string>("TokenRedisConnectionString"));
            configurationOptions.CertificateValidation += (request, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
            _database = _connectionMultiplexer.GetDatabase();
        }
        public async Task Add(string key, CommonTokenModel value, TimeSpan expiry)
        {
            value.Expiration = DateTime.Now.Add(expiry);
            var reuslt = _database.StringSet(key, JsonSerializer.SerializeToUtf8Bytes(value), expiry);
            await Task.FromResult(reuslt);
        }
        public async Task<bool> Exists(string key) 
        {
            var reuslt = _database.KeyExists(key);
            return await Task.FromResult(reuslt);
        }
        public async Task<CommonTokenModel> Get(string key) 
        {;
            var result = JsonSerializer.Deserialize<CommonTokenModel>(_database.StringGet(key));
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
            var result = _database.KeyDelete(key);
            await Task.FromResult(result);
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
