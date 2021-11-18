using NetCoreProject.BusinessLayer;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.DataLayer;
using NetCoreProject.DataLayer.IManager;
using NetCoreProject.Domain.AbstractInterceptor;
using NetCoreProject.Domain.Config;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Service;
using NetCoreProject.Domain.Util;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;
using NUnit.Framework;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System;

namespace NetCoreProject.NUnit
{
    public class NUnitTest
    {
        private readonly IHost _host;
        public NUnitTest()
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            _host = new HostBuilder()
            .ConfigureAppConfiguration((hostBuilder, configurationBuilder) =>
            {
                var environment = hostBuilder.HostingEnvironment;
                configurationBuilder
                    .SetBasePath(environment.ContentRootPath)
                    .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging((hostBuilder, builder) =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
            })
            .UseNLog()
            .ConfigureServices((hostBuilder, services) =>
            {
                var configuration = hostBuilder.Configuration;
                services.AddSingleton(provider => configuration);
                services.AddOptions();
                // DbContext
                {
                    services.AddDbContext<DefaultDbContext>(option =>
                    {
                        var connectionName = "Default";
                        option.UseInMemoryDatabase(databaseName: connectionName);
                        //option.UseSqlServer(SecurityUtil.Decrypt(configuration.GetConnectionString(connectionName), securityKey),
                        //option.UseSqlServer(configuration.GetConnectionString(connectionName),
                        //    sqlServerOption =>
                        //    {
                        //        sqlServerOption.MinBatchSize(10);
                        //        sqlServerOption.MaxBatchSize(1000);
                        //        sqlServerOption.CommandTimeout(configuration.GetValue<int>($"ConnectionTimeoutConfig:{connectionName}"));
                        //    });
                        option.EnableSensitiveDataLogging();
                        option.EnableDetailedErrors();
                    });
                }
                services.AddSingleton<IMemoryCache>(factory =>
                {
                    var cache = new MemoryCache(new MemoryCacheOptions());
                    return cache;
                });
                services.AddSingleton<ICaptchaCacheService, CaptchaMemoryCacheService>();
                services.AddSingleton<ITokenCacheService, TokenMemoryCacheService>();
                services.AddDataProtection();
                services.AddScoped<IDefaultDataProtector, DefaultDataProtector>();
                services.AddScoped<IDapperService<DefaultDbContext>, DapperService<DefaultDbContext>>();
                services.AddScoped<ISqlBulkCopyService<DefaultDbContext>, SqlBulkCopyService<DefaultDbContext>>();
                //services.AddScoped<IUserService, UserBatchService>();
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
            })
            .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory())
            .Build();
        }
        protected IHost GetHost() => this._host;
    }
}