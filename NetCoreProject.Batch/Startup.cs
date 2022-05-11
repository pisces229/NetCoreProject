using NetCoreProject.BusinessLayer;
using NetCoreProject.DataLayer;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.AbstractInterceptor;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Service;
using NetCoreProject.Domain.Util;
using NetCoreProject.Batch.Runner;
using NetCoreProject.Batch.Mapper;
using NetCoreProject.Batch.Service;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using NLog.Extensions.Hosting;
using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace NetCoreProject.Batch
{
    public class Startup
    {
        private readonly string _environmentName;
        private readonly string _batchProid;
        private readonly string _batchSeqno;
        private readonly IHost _host;
        public Startup()
        {
            foreach (var commandLineArg in Environment.GetCommandLineArgs())
            {
                var values = commandLineArg.Split('|');
                switch (values.FirstOrDefault())
                {
                    case "ASPNETCORE_ENVIRONMENT":
                        _environmentName = values.Skip(1).FirstOrDefault();
                        break;
                    case "BATCH_PROID":
                        _batchProid = values.Skip(1).FirstOrDefault();
                        break;
                    case "BATCH_SEQNO":
                        _batchSeqno = values.Skip(1).FirstOrDefault();
                        break;
                }
            }
            Environment.SetEnvironmentVariable("NETOCRESAMPLEPROJECT_BATCH_PROID", _batchProid);
            Environment.SetEnvironmentVariable("NETOCRESAMPLEPROJECT_BATCH_SEQNO", _batchSeqno);
            string appsettingsJsonName;
            string nlogConfigName;
            switch (_environmentName)
            {
                case "Development":
                    appsettingsJsonName = "appsettings.json";
                    nlogConfigName = "nlog.config";
                    //nlogConfigName = "nlog.Seq.config";
                    break;
                case "Test":
                    appsettingsJsonName = "appsettings.Test.json";
                    nlogConfigName = "nlog.Test.config";
                    break;
                case "Production":
                    appsettingsJsonName = "appsettings.Production.json";
                    nlogConfigName = "nlog.Production.config";
                    break;
                default:
                    throw new Exception("Unknow EnvironmentName");
            }

            _host = new HostBuilder()
                .ConfigureAppConfiguration((hostBuilder, configurationBuilder) =>
                {
                    var environment = hostBuilder.HostingEnvironment;
                    configurationBuilder
                        .SetBasePath(environment.ContentRootPath)
                        .AddJsonFile(path: appsettingsJsonName, optional: false, reloadOnChange: true);

                    NLog.LogManager.LoadConfiguration(nlogConfigName);
                    // LogManager.Configuration.Variables["name"] = "value";

                    var logger = NLog.LogManager.GetLogger("Startup");
                    logger.Info($"CommandLineArgs:{ string.Join(" ", Environment.GetCommandLineArgs()) }");
                    logger.Info($"EnvironmentName:{ _environmentName }");
                    logger.Info($"appsettingsJsonName:{ appsettingsJsonName }");
                    logger.Info($"nlogConfigName:{ nlogConfigName }");
                    logger.Info($"batchProid:{ _batchProid }");
                    logger.Info($"batchSeqno:{ _batchSeqno }");
                })
                .ConfigureLogging((hostBuilder, builder) =>
                {
                    //builder.AddConsole(options => options.IncludeScopes = true);
                    if ("Development".Equals(_environmentName))
                    {
                        builder.SetMinimumLevel(LogLevel.Information);
                    }
                })
                .UseNLog()
                .ConfigureServices((hostBuilder, services) =>
                {
                    var configuration = hostBuilder.Configuration;
                    services.AddSingleton(provider => configuration);
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
                            option.UseSqlServer(configuration.GetConnectionString(connectionName),
                                sqlServerOption =>
                                {
                                    sqlServerOption.MinBatchSize(10);
                                    sqlServerOption.MaxBatchSize(1000);
                                    sqlServerOption.CommandTimeout(configuration.GetValue<int>($"ConnectionTimeoutConfig:{connectionName}"));
                                });
                            //option.UseLoggerFactory();
                            if ("Development".Equals(_environmentName))
                            {
                                option.EnableSensitiveDataLogging();
                            }
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
                    services.AddScoped<IUserService, UserBatchService>();
                    services.AddSingleton<SqlUtil>();
                    services.AddSingleton<CompressUtil>();
                    services.AddSingleton<ConfigurationUtil>();
                    services.AddSingleton<ConvertValueUtil>();
                    services.AddSingleton<FileUtil>();
                    services.AddSingleton<ValidateMessageUtil>();
                    services.AddSingleton<ValidateValueUtil>();
                    LogicConfigure.AddService(services);
                    ManagerConfigure.AddService(services);
                    AddService(services);
                    services.ConfigureDynamicProxy(configure => {
                        // Interceptors
                        // name(...), prefix (...*), suffix (*...)
                        // configure.Interceptors.AddTyped<DefaultAopAttribute>(); 
                        // Predicates.ForNameSpace, Predicates.ForService, Predicates.ForMethod
                        // configure.Interceptors.AddTyped<DefaultAopAttribute>(Predicates.ForNameSpace("..."));
                        // NonAspectPredicates
                        // name(...), prefix (...*), suffix (*...)
                        // configure.NonAspectPredicates.AddNamespace("...");
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
                        CreateMap(configure);
                    });
                })
                .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory())
                //.UseConsoleLifetime()
                //.UseEnvironment(EnvironmentName.Development)
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.RunConsoleAsync();
                .Build();
        }
        public void AddService(IServiceCollection services)
        {
            switch (_batchProid)
            {
                case "Test":
                    services.AddScoped<IRunner, TestRunner>();
                    break;
            }
            
        }
        public void CreateMap(IMapperConfigurationExpression mapperConfiguration)
        {
            switch (_batchProid)
            {
                case "Test":
                    TestRunnerMapperConfiguration.CreateMap(mapperConfiguration);
                    break;
            }
        }
        public async Task Excute()
        {
            await _host.Services.GetRequiredService<IRunner>().Execute();
        }
    }
}
