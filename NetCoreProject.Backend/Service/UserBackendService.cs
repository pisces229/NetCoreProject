using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace NetCoreProject.Backend.Service
{
    public class UserBackendService : IUserService
    {
        private readonly CommonUserModel _commonUserModel;
        public UserBackendService(IHttpContextAccessor httpContextAccessor,
            ILogger<UserBackendService> logger,
            ITokenCacheService tokenCacheService,
            IOptions<JwtConfig> jwtConfigOptions)
        {
            _commonUserModel = new CommonUserModel()
            {
                Proid = "",
                Userid = "_",
                Username = "_"
            };
            try
            {
                var headerKeys = httpContextAccessor.HttpContext.Request.Headers.Keys;
                if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var headerValue = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                    var values = headerValue.Split(' ');
                    if (values.Length == 2)
                    {
                        if ("Bearer".Equals(values.First()))
                        {
                            var jwtConfig = jwtConfigOptions.Value;
                            var token = values.Last();
                            var tokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = jwtConfig.Issuer,
                                ValidateAudience = true,
                                ValidAudience = jwtConfig.Audience,
                                ValidateLifetime = false,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = jwtConfig.SecurityKey
                            };
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                            var refreshTokenId = claimsPrincipal.Claims
                                .Where(w => w.Type == JwtRegisteredClaimNames.Jti)
                                .Select(s => s.Value)
                                .FirstOrDefault();
                            if (!string.IsNullOrEmpty(refreshTokenId))
                            {
                                if (tokenCacheService.Exists(refreshTokenId).Result)
                                {
                                    var cmmonTokenModel = tokenCacheService.Get(refreshTokenId).Result;
                                    _commonUserModel.Userid = cmmonTokenModel.Username;
                                    _commonUserModel.Username = cmmonTokenModel.Username;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }
        public string Proid() => _commonUserModel.Proid;
        public string Userid() => _commonUserModel.Userid;
        public string Username() => _commonUserModel.Username;
    }
}
