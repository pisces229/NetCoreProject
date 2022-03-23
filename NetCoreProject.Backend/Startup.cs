extern alias StackExchangeRedis;

using NetCoreProject.BusinessLayer;
using NetCoreProject.DataLayer;
using NetCoreProject.Domain.AbstractInterceptor;
using NetCoreProject.Domain.Config;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Service;
using NetCoreProject.Domain.Util;
using NetCoreProject.Backend.AuthorizationFilter;
using NetCoreProject.Backend.ResourceFilter;
using NetCoreProject.Backend.ActionFilter;
using NetCoreProject.Backend.ResultFilter;
using NetCoreProject.Backend.ExceptionFilter;
using NetCoreProject.Backend.Middleware;
using NetCoreProject.Backend.Service;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using StackExchangeRedis::StackExchange.Redis;
using NLog.Web;
using System.Net.Security;
using NetCoreProject.Domain.Enum;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using NetCoreProject.Backend.Csp;

namespace NetCoreProject.Backend
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _environmentName;
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _environmentName = _webHostEnvironment.EnvironmentName;
            string appsettingsJsonName;
            string nlogConfigName;
            switch (_environmentName)
            {
                case "Development":
                    appsettingsJsonName = "appsettings.json";
                    nlogConfigName = "nlog.config";
                    //nlogConfigName = "nlog.Elasticsearch.config";
                    //nlogConfigName = "nlog.Seq.config";
                    break;
                case "Test":
                    appsettingsJsonName = "appsettings.Test.json";
                    nlogConfigName = "nlog.Test.config";
                    //nlogConfigName = "nlog.Elasticsearch.config";
                    break;
                case "Production":
                    appsettingsJsonName = "appsettings.Production.json";
                    nlogConfigName = "nlog.Production.config";
                    break;
                default:
                    throw new Exception("Unknow EnvironmentName");
            }
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(_webHostEnvironment.ContentRootPath)
                .AddJsonFile(path: appsettingsJsonName, optional: false, reloadOnChange: true);
            _configuration = configurationBuilder.Build();

            NLogBuilder.ConfigureNLog(nlogConfigName);

            var logger = LogManager.GetLogger("Startup");
            logger.Info($"EnvironmentName:{ _environmentName }");
            logger.Info($"appsettingsJsonName:{ appsettingsJsonName }");
            logger.Info($"nlogConfigName:{ nlogConfigName }");
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => _configuration);
            services.AddCors(options =>
            {
                // CorsPolicy 是自訂的 Policy 名稱
                options.AddPolicy("CorsPolicy", policy =>
                {
                    //policy.AllowAnyOrigin()
                    //    .AllowAnyHeader()
                    //    .AllowAnyMethod()
                    //    .AllowCredentials();
                    //policy.WithOrigins("https://localhost:44387/", "")
                    //    .WithHeaders("", "")
                    //    .WithMethods("", "");
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                    // content-disposition
                        .WithExposedHeaders("content-disposition");
                    // x-download-filename
                    //  .WithExposedHeaders("x-download-filename");
                });
            });
            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(60);
            //    options.ExcludedHosts.Add("example.com");
            //    options.ExcludedHosts.Add("www.example.com");
            //});
            //if (!_webHostEnvironment.IsDevelopment())
            //{
            //    services.AddHttpsRedirection(options =>
            //    {
            //        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //        options.HttpsPort = 443;
            //    });
            //}
            // 註冊 Options Pattern 服務，將配置內容註冊到容器裡，來獲取對應的服務 Provider 對象
            services.AddOptions();
            // DbContext
            {
                services.AddDbContext<DefaultDbContext>(option =>
                {
                    var connectionName = "Default";
                    // UseInMemoryDatabase
                    //option.UseInMemoryDatabase(databaseName: connectionName);
                    //option.UseSqlServer(SecurityUtil.Decrypt(_configuration.GetConnectionString(connectionName), securityKey),
                    option.UseSqlServer(_configuration.GetConnectionString(connectionName),
                        sqlServerOption =>
                        {
                            sqlServerOption.MinBatchSize(10);
                            sqlServerOption.MaxBatchSize(1000);
                            sqlServerOption.CommandTimeout(_configuration.GetValue<int>($"ConnectionTimeoutConfig:{connectionName}"));
                        });
                    //option.UseLoggerFactory();
                    if (_webHostEnvironment.IsDevelopment())
                    {
                        option.EnableSensitiveDataLogging();
                    }
                    option.EnableDetailedErrors();
                });
                services.AddDbContext<DataProtectionKeyContext>(option =>
                {
                    var connectionName = "Redis";
                    // UseInMemoryDatabase
                    option.UseInMemoryDatabase(databaseName: connectionName);
                    //option.UseSqlServer(SecurityUtil.Decrypt(_configuration.GetConnectionString(connectionName), securityKey),
                    //option.UseSqlServer(_configuration.GetConnectionString(connectionName),
                    //    sqlServerOption =>
                    //    {
                    //        sqlServerOption.CommandTimeout(connectionTimeoutConfig.GetValue<int>(connectionName));
                    //    });
                    //option.UseLoggerFactory();
                    if (_webHostEnvironment.IsDevelopment())
                    {
                        option.EnableSensitiveDataLogging();
                    }
                    option.EnableDetailedErrors();
                });
            }
            // Cache
            {
                switch (_configuration.GetValue<int>("CacheType"))
                {
                    case 0:
                        {
                            services.AddSingleton<IMemoryCache>(factory =>
                            {
                                var cache = new MemoryCache(new MemoryCacheOptions());
                                return cache;
                            });
                            services.AddSingleton<ICaptchaCacheService, CaptchaMemoryCacheService>();
                            services.AddSingleton<ITokenCacheService, TokenMemoryCacheService>();
                        }
                        break;
                    case 1:
                        {
                            services.AddSingleton<ICaptchaCacheService, CaptchaRedisCacheService>();
                            services.AddSingleton<ITokenCacheService, TokenRedisCacheService>();
                        }
                        break;
                    case 2:
                        {
                            services.AddDistributedSqlServerCache(options =>
                            {
                                options.ConnectionString = _configuration.GetConnectionString("Redis");
                                options.SchemaName = "dbo";
                                options.TableName = "DataCache";
                                options.ExpiredItemsDeletionInterval = TimeSpan.FromMinutes(5);
                            });
                            services.AddSingleton<ICaptchaCacheService, CaptchaSqlServerCacheService>();
                            services.AddSingleton<ITokenCacheService, TokenSqlServerCacheService>();
                        }
                        break;
                }
            }
            // Authorization
            {
                var jwtSigning = _configuration.GetSection("JwtSigning");
                var jwtType = jwtSigning.GetValue<int>("Type");
                var symmetricSecurityKey = default(SymmetricSecurityKey);
                var publicRsaSecurityKey = default(RsaSecurityKey);
                var privateRsaSecurityKey = default(RsaSecurityKey);
                switch (jwtType)
                {
                    case (int)JwtTypeEnum.String:
                        {
                            symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSigning.GetValue<string>("StringSecretKey")));
                        }
                        break;
                    case (int)JwtTypeEnum.Pem:
                        {
                            var rsaPublicKey = RSA.Create();
                            var pemCertificateContents = File.ReadAllText(jwtSigning.GetValue<string>("PemPublicFile"));
                            var pemCertificateHeader = "-----BEGIN PUBLIC KEY-----";
                            var pemCertificateFooter = "-----END PUBLIC KEY-----";
                            var endIdx = pemCertificateContents.IndexOf(
                                pemCertificateFooter,
                                pemCertificateHeader.Length,
                                StringComparison.Ordinal);
                            var base64 = pemCertificateContents.Substring(
                                pemCertificateHeader.Length,
                                endIdx - pemCertificateHeader.Length);
                            var binary = Convert.FromBase64String(base64);
                            rsaPublicKey.ImportSubjectPublicKeyInfo(binary, out _);
                            publicRsaSecurityKey = new RsaSecurityKey(rsaPublicKey.ExportParameters(false));
                        }
                        {
                            var rsaPrivateKey = RSA.Create();
                            var pemPrivateContents = File.ReadAllText(jwtSigning.GetValue<string>("PemPrivateFile"));
                            var pemPrivateKeyHeader = "-----BEGIN PRIVATE KEY-----";
                            var pemPrivateKeyFooter = "-----END PRIVATE KEY-----";
                            var endIdx = pemPrivateContents.IndexOf(
                                pemPrivateKeyFooter,
                                pemPrivateKeyHeader.Length,
                                StringComparison.Ordinal);
                            var base64 = pemPrivateContents.Substring(
                                pemPrivateKeyHeader.Length,
                                endIdx - pemPrivateKeyHeader.Length);
                            var binary = Convert.FromBase64String(base64);
                            rsaPrivateKey.ImportPkcs8PrivateKey(binary, out _);
                            privateRsaSecurityKey = new RsaSecurityKey(rsaPrivateKey.ExportParameters(true));
                        }
                        break;
                    case (int)JwtTypeEnum.Pfx:
                        {
                            var pfxFile = jwtSigning.GetValue<string>("PfxFile");
                            var pfxPassword = jwtSigning.GetValue<string>("PfxPassword");
                            var publicRsaKeyParameters = (RsaKeyParameters)CertUtil.GetPublicKey(pfxFile, pfxPassword).GetPublicKey();
                            publicRsaSecurityKey = new RsaSecurityKey(new RSAParameters()
                            {
                                Exponent = publicRsaKeyParameters.Exponent.ToByteArrayUnsigned(),
                                Modulus = publicRsaKeyParameters.Modulus.ToByteArrayUnsigned()
                            });
                            var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)CertUtil.GetPrivateKey(pfxFile, pfxPassword);
                            privateRsaSecurityKey = new RsaSecurityKey(DotNetUtilities.ToRSAParameters(rsaPrivateCrtKeyParameters));
                        }
                        break;
                }
                var jwtConfig = _configuration.GetSection(nameof(JwtConfig));
                services.Configure<JwtConfig>(options =>
                {
                    options.NameClaimType = jwtConfig.GetValue<string>(nameof(JwtConfig.NameClaimType));
                    options.RoleClaimType = jwtConfig.GetValue<string>(nameof(JwtConfig.RoleClaimType));
                    options.Issuer = jwtConfig.GetValue<string>(nameof(JwtConfig.Issuer));
                    options.Subject = jwtConfig.GetValue<string>(nameof(JwtConfig.Subject));
                    options.Audience = jwtConfig.GetValue<string>(nameof(JwtConfig.Audience));
                    options.ValidFor = TimeSpan.FromSeconds(jwtConfig.GetValue<double>(nameof(JwtConfig.ValidFor)));
                    switch (jwtType)
                    {
                        case (int)JwtTypeEnum.String:
                            options.SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
                            options.SecurityKey = symmetricSecurityKey;
                            break;
                        case (int)JwtTypeEnum.Pem:
                            options.SigningCredentials = new SigningCredentials(privateRsaSecurityKey, SecurityAlgorithms.RsaSha256)
                            {
                                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                            };
                            options.SecurityKey = publicRsaSecurityKey;
                            break;
                        case (int)JwtTypeEnum.Pfx:
                            options.SigningCredentials = new SigningCredentials(privateRsaSecurityKey, SecurityAlgorithms.RsaSha256);
                            options.SecurityKey = publicRsaSecurityKey;
                            break;
                    }
                });
                // AddAuthorization
                services.AddAuthorization(options =>
                {
                    // 設定驗證原則
                    // 這裡僅驗證是否有指定角色
                    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                    options.AddPolicy("Member", policy => policy.RequireRole("Member"));
                });
                // AddAuthentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                    // 預設值為 true，有時會特別關閉
                    options.IncludeErrorDetails = true;
                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = jwtConfig.GetValue<string>(nameof(JwtConfig.NameClaimType)),
                        RoleClaimType = jwtConfig.GetValue<string>(nameof(JwtConfig.RoleClaimType)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtConfig.GetValue<string>(nameof(JwtConfig.Issuer)),
                        ValidateAudience = true,
                        ValidAudience = jwtConfig.GetValue<string>(nameof(JwtConfig.Audience)),
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                    switch (jwtType)
                    {
                        case (int)JwtTypeEnum.String:
                            tokenValidationParameters.IssuerSigningKey = symmetricSecurityKey;
                            break;
                        case (int)JwtTypeEnum.Pem:
                            tokenValidationParameters.IssuerSigningKey = publicRsaSecurityKey;
                            break;
                        case (int)JwtTypeEnum.Pfx:
                            tokenValidationParameters.IssuerSigningKey = publicRsaSecurityKey;
                            break;
                    }
                    options.TokenValidationParameters = tokenValidationParameters;
                });
            }
            // DataProtection
            {
                var configurationSection = _configuration.GetSection("DataProtection");
                var dataProtectionBuilder = services.AddDataProtection()
                    .SetApplicationName(configurationSection.GetValue<string>("ApplicationName"));
                if (configurationSection.GetValue<bool>("AutomaticKeyGeneration"))
                {
                    dataProtectionBuilder.SetDefaultKeyLifetime(TimeSpan.FromDays(configurationSection.GetValue<int>("KeyLifetime")));
                }
                else
                {
                    dataProtectionBuilder.DisableAutomaticKeyGeneration();
                }
                switch (configurationSection.GetValue<int>("Type"))
                {
                    case 1:
                        var configurationOptions = ConfigurationOptions.Parse(_configuration.GetValue<string>("DefaultRedisConnectionString"));
                        configurationOptions.CertificateValidation += (request, certificate, chain, sslPolicyErrors) =>
                        {
                            return true;
                        };
                        var connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
                        dataProtectionBuilder.PersistKeysToStackExchangeRedis(connectionMultiplexer, "DataProtection-Keys");
                        break;
                    case 2:
                        dataProtectionBuilder.PersistKeysToDbContext<DataProtectionKeyContext>();
                        break;
                }
            }
            services.AddScoped<DefaultAuthorizationFilter>();
            services.AddScoped<IDefaultDataProtector, DefaultDataProtector>();
            services.AddScoped<IDapperService<DefaultDbContext>, DapperService<DefaultDbContext>>();
            services.AddScoped<ISqlBulkCopyService<DefaultDbContext>, SqlBulkCopyService<DefaultDbContext>>();
            services.AddScoped<IUserService, UserBackendService>();
            services.AddSingleton<SqlUtil>();
            services.AddSingleton<CompressUtil>();
            services.AddSingleton<ConfigurationUtil>();
            services.AddSingleton<ConvertValueUtil>();
            services.AddSingleton<FileUtil>();
            services.AddSingleton<ValidateMessageUtil>();
            services.AddSingleton<ValidateValueUtil>();
            LogicConfigure.AddService(services);
            ManagerConfigure.AddService(services);
            services.ConfigureDynamicProxy(configure => {
                // Interceptors
                // name(...), prefix (...*), suffix (*...)
                // configure.Interceptors.AddTyped<DefaultAopAttribute>(); 
                // Predicates.ForNameSpace, Predicates.ForService, Predicates.ForMethod
                // configure.Interceptors.AddTyped<DefaultAopAttribute>(Predicates.ForNameSpace("..."));
                // NonAspectPredicates
                // name(...), prefix (...*), suffix (*...)
                // configure.NonAspectPredicates.AddNamespace("...");
                //configure.Interceptors.AddTyped<DefaultAopAttribute>();
                configure.Interceptors.AddTyped<InputAopAttribute>(
                    Predicates.ForNameSpace("NetCoreProject.BusinessLayer.ILogic"),
                    Predicates.ForNameSpace("NetCoreProject.DataLayer.IManager"));
                configure.Interceptors.AddTyped<TimerAopAttribute>(
                    Predicates.Implement(typeof(ISqlBulkCopyService<DefaultDbContext>)));
                configure.Interceptors.AddTyped<DapperAopAttribute>(
                    Predicates.Implement(typeof(IDapperService<DefaultDbContext>)));
            });
            services.AddAspectServiceContext();
            services.AddAutoMapper(configure => {
                //configure.AllowNullDestinationValues = false;
                //configure.CreateMap<T1, T2>();
                LogicConfigure.CreateMap(configure);
            });
            services.AddControllers().AddJsonOptions(
                options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add(typeof(DefaultAuthorizationFilter));
                //options.Filters.Add(typeof(DefaultResourceFilter));
                //options.Filters.Add(typeof(DefaultActionFilter));
                //options.Filters.Add(typeof(DefaultResultFilter));
                options.Filters.Add(typeof(DefaultExceptionFilter));
            });
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("CorsPolicy");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                //app.UseHttpsRedirection();
            }
            // before UseRouting
            app.UseDefaultFiles();
            //app.UseStaticFiles();
            app.UseSpaStaticFiles();
            // need UseRouting
            app.UseRouting();
            app.UseMiddleware<DefaultMiddleware>();
            // before Middleware
            //app.UseCors();
            // before UseEndpoints
            app.UseAuthentication();
            app.UseAuthorization();
            // Content-Security-Policy
            app.UseCsp(options =>
            {
                options.Scripts.AllowSelf().UnsafeInline().UnsafeEval();
                options.Styles.AllowSelf().UnsafeInline();
                options.Images.AllowSelf().Allow("data:");
                options.Frames.Disallow();
            });
            // X-Frame-Options
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                await next();
            });
            // X-Content-Type-Options
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });
            //app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api/"), builder =>
            //{
            //    builder.UseSpa(spa =>
            //    {
            //        spa.Options.SourcePath = "wwwroot";
            //    });
            //});
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action}");
            });
        }
    }
}
