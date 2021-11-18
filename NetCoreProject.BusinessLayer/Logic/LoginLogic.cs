using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Config;
using NetCoreProject.Domain.IService;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Model.Login;
using NetCoreProject.DataLayer.IManager;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Linq;
using NetCoreProject.Domain.Enum;

namespace NetCoreProject.BusinessLayer.Logic
{
    public class LoginLogic : ILoginLogic
    {
        private readonly ILogger<LoginLogic> _logger;
        private readonly IMapper _mapper;
        private readonly JwtConfig _jwtConfig;
        private readonly IUserService _userService;
        private readonly ILoginManager _loginManager;
        public LoginLogic(ILogger<LoginLogic> logger,
            IMapper mapper,
            IOptions<JwtConfig> jwtConfig,
            IUserService userService,
            ILoginManager loginManager)
        {
            _logger = logger;
            _mapper = mapper;
            _jwtConfig = jwtConfig.Value;
            _userService = userService;
            _loginManager = loginManager;
        }
        public async Task<CommonApiResultModel<string>> SignIn(LoginSignInInputModel model)
        {
            var result = new CommonApiResultModel<string>();
            var username = _userService.Username();
            if (await _loginManager.ValidateUser(model.Username))
            {
                var refreshTokenId = Guid.NewGuid().ToString();
                var token = await GenerateToken(model.Username, refreshTokenId);
                var commonTokenModel = new CommonTokenModel()
                {
                    Username = model.Username
                };
                await _loginManager.SaveToken(refreshTokenId, commonTokenModel);
                result.Success = true;
                result.Data = token;
            }
            else
            {
                result.Success = false;
                result.Message = "Login Fail";
            }
            return result;
        }
        public async Task<string> Refresh(string model)
        {
            var result = "";
            try
            {
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
                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(model, tokenValidationParameters, out _);
                claimsPrincipal.Claims.ToList().ForEach(f =>
                {
                    _logger.LogInformation($"{f.Type}:{f.Value}");
                });
                var userName = claimsPrincipal.Claims
                    .Where(w => w.Type == _jwtConfig.NameClaimType)
                    .Select(s => s.Value)
                    .FirstOrDefault();
                var refreshTokenId = claimsPrincipal.Claims
                    .Where(w => w.Type == JwtRegisteredClaimNames.Jti)
                    .Select(s => s.Value)
                    .FirstOrDefault();
                if (await _loginManager.ValidateToken(refreshTokenId))
                {
                    var token = await GenerateToken(userName, refreshTokenId);
                    var commonTokenModel = new CommonTokenModel()
                    {
                        Username = userName
                    };
                    await _loginManager.SaveToken(refreshTokenId, commonTokenModel);
                    result = token;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return result;
        }
        public async Task SignOut(string model)
        {
            try
            {
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
                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(model, tokenValidationParameters, out _);
                claimsPrincipal.Claims.ToList().ForEach(f =>
                {
                    Console.WriteLine($"{f.Type}:{f.Value}");
                });
                var userName = claimsPrincipal.Claims
                    .Where(w => w.Type == _jwtConfig.NameClaimType)
                    .Select(s => s.Value)
                    .FirstOrDefault();
                var refreshTokenId = claimsPrincipal.Claims
                    .Where(w => w.Type == _jwtConfig.NameClaimType)
                    .Select(s => s.Value)
                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(refreshTokenId))
                {
                    await _loginManager.RemoveToken(refreshTokenId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
        private Task<string> GenerateToken(string userName, string refreshTokenId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Iss, _jwtConfig.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience),
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Exp, _jwtConfig.Expiration.Ticks.ToString()),
                //new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.Ticks.ToString()),
                //new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.Ticks.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, refreshTokenId),
                //claims.Add(new Claim(ClaimTypes.Name, userName));
                // 你可以自行擴充 "" 加入登入者該有的角色
                new Claim("roles", "Admin"),
                //new Claim("roles", "Users")
            };
            var claimsIdentity = new ClaimsIdentity(claims);
            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                //NotBefore = DateTime.Now,
                //IssuedAt = DateTime.Now,
                Subject = claimsIdentity,
                Expires = _jwtConfig.Expiration,
                SigningCredentials = _jwtConfig.SigningCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);
            return Task.FromResult(serializeToken);
        }
    }
}
