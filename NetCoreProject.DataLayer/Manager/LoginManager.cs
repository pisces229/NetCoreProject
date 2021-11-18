using NetCoreProject.DataLayer.IManager;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NetCoreProject.DataLayer.Manager
{
    public class LoginManager : ILoginManager
    {
        private readonly ILogger<LoginManager> _logger;
        private readonly ICaptchaCacheService _captchaCacheService;
        private readonly ITokenCacheService _tokenCacheService;
        public LoginManager(ILogger<LoginManager> logger,
            ICaptchaCacheService captchaCacheService,
            ITokenCacheService tokenCacheService)
        {
            _logger = logger;
            _captchaCacheService = captchaCacheService;
            _tokenCacheService = tokenCacheService;
        }
        public async Task<bool> ValidateUser(string username)
        {
            return await Task.FromResult(true);
        }
        public async Task<CommonTokenModel> GetToken(string token)
        {
            return await _tokenCacheService.Get(token);
        }
        public async Task SaveToken(string token, CommonTokenModel value)
        {
            await _tokenCacheService.Add(token, value, TimeSpan.FromMinutes(5));
        }
        public async Task<bool> ValidateToken(string token)
        {
            return await _tokenCacheService.Exists(token);
        }
        public async Task UpdateToken(string token, CommonTokenModel value)
        {
            await _tokenCacheService.Replace(token, value, TimeSpan.FromMinutes(5));
        }
        public async Task RemoveToken(string token)
        {
            await _tokenCacheService.Remove(token);
        }
    }
}
