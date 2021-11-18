using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NetCoreProject.Domain.Enum;

namespace NetCoreProject.Backend.AuthorizationFilter
{
    public class DefaultAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly ILogger<DefaultAuthorizationFilter> _logger;
        private readonly ITokenCacheService _tokenCacheService;
        private readonly JwtConfig _jwtConfig;
        public DefaultAuthorizationFilter(ILogger<DefaultAuthorizationFilter> logger,
            ITokenCacheService tokenCacheService,
            IOptions<JwtConfig> jwtConfig)
        {
            _logger = logger;
            _tokenCacheService = tokenCacheService;
            _jwtConfig = jwtConfig.Value;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await Task.Run(() => _logger.LogInformation("DefaultAuthorizationFilter"));
            var headerKeys = context.HttpContext.Request.Headers.Keys;
            var httpStatusCode = HttpStatusCode.Forbidden;
            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    var headerValue = context.HttpContext.Request.Headers["Authorization"].ToString();
                    var values = headerValue.Split(' ');
                    if (values.Length == 2)
                    {
                        if ("Bearer".Equals(values.First()))
                        {
                            var token = values.Last();
                            var tokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = _jwtConfig.Issuer,
                                ValidateAudience = true,
                                ValidAudience = _jwtConfig.Audience,
                                ValidateLifetime = false,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = _jwtConfig.SecurityKey
                            };
                            var securityToken = default(SecurityToken);
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                            // securityToken.ValidFrom;
                            // securityToken.ValidTo;
                            if (securityToken.ValidTo > DateTime.UtcNow)
                            {
                                var refreshTokenId = claimsPrincipal.Claims
                                    .Where(w => w.Type == JwtRegisteredClaimNames.Jti)
                                    .Select(s => s.Value)
                                    .FirstOrDefault();
                                if (await _tokenCacheService.Exists(refreshTokenId))
                                {
                                    httpStatusCode = HttpStatusCode.OK;
                                }
                            }
                            else
                            {
                                httpStatusCode = HttpStatusCode.Unauthorized;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
            if (httpStatusCode == HttpStatusCode.Unauthorized)
            {
                context.Result = new UnauthorizedResult();
            }
            else if (httpStatusCode == HttpStatusCode.Forbidden)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
