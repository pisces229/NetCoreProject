using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreProject.Backend.Controller;
using NetCoreProject.BusinessLayer;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Logic;
using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.DataLayer.IManager;
using NetCoreProject.DataLayer.Manager;
using NetCoreProject.DataLayer.Model.Test;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Util;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NetCoreProject.NSubstitute
{
    public class UnitTest_Test
    {
        private readonly ILogger<TestController> _loggerTestController;
        private readonly ILogger<TestLogic> _loggerTestLogic;
        private readonly ILogger<TestManager> _loggerTestManager;
        private readonly DefaultDbContext _defaultDbContext;
        private readonly IDefaultDataProtector _defaultDataProtector;
        private readonly IMapper _mapper;
        private readonly SqlUtil _sqlUtil;
        private readonly ConfigurationUtil _configurationUtil;
        public UnitTest_Test()
        {
            var service = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .BuildServiceProvider();
            _loggerTestController = service.GetService<ILoggerFactory>().CreateLogger<TestController>();
            _loggerTestLogic = service.GetService<ILoggerFactory>().CreateLogger<TestLogic>();
            _loggerTestManager = service.GetService<ILoggerFactory>().CreateLogger<TestManager>();
            var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "DefaultDbContext");
            _defaultDbContext = new DefaultDbContext(optionsBuilder.Options);
            _defaultDataProtector = Substitute.For<IDefaultDataProtector>();
            _defaultDataProtector.Protect(Arg.Any<string>()).Returns(r => r.ArgAt<string>(0));
            _defaultDataProtector.Unprotect(Arg.Any<string>()).Returns(r => r.ArgAt<string>(0));
            _mapper = new Mapper(new MapperConfiguration(c => LogicConfigure.CreateMap(c)));
            _sqlUtil = new SqlUtil();
            _configurationUtil = new ConfigurationUtil(Utility.CreateConfiguration());
        }
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public async Task Test_TestController_QueryGrid()
        {
            var testLogic = Substitute.For<ITestLogic>();
            testLogic.QueryGrid(
                Arg.Any<CommonQueryPageModel<TestLogicQueryGridInputModel>>())
            .Returns(Task.FromResult(new CommonApiResultModel<CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>>()
            {
                Success = true,
                Data = new CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>()
            }));

            var testController = new TestController(_loggerTestController,
                testLogic,
                _configurationUtil);
            var input = new CommonQueryPageModel<TestLogicQueryGridInputModel>()
            {
                Data = new TestLogicQueryGridInputModel()
                {
                    NAME = "Mock"
                },
                Page = new CommonPageModel()
                {
                }
            };
            var output = await testController.QueryGrid(input);

            await testLogic.Received(1).QueryGrid(
                Arg.Any<CommonQueryPageModel<TestLogicQueryGridInputModel>>());
            Assert.NotNull(output);

            Console.WriteLine(output);
        }
        [Test]
        public async Task Test_TestLogic_QueryGrid()
        {
            var testManager = Substitute.For<ITestManager>();
            testManager.QueryGrid(
                Arg.Any<TestManagerQueryGridModel>(),
                Arg.Any<CommonPageModel>())
            .Returns(Task.FromResult(new CommonQueryPageResultModel<TestManagerQueryDto>()
            {
                Data = new List<TestManagerQueryDto>()
                    {
                        new TestManagerQueryDto()
                        {
                            ROW = 1,
                            NAME = "Mock"
                        }
                    },
                Page = new CommonPageModel()
            }));

            var testLogic = new TestLogic(_loggerTestLogic,
                _defaultDataProtector,
                _defaultDbContext,
                _mapper,
                testManager,
                _configurationUtil);
            var input = new CommonQueryPageModel<TestLogicQueryGridInputModel>()
            {
                Data = new TestLogicQueryGridInputModel()
                {
                    NAME = "Mock"
                },
                Page = new CommonPageModel()
                {
                }
            };
            var output = await testLogic.QueryGrid(input);

            await testManager.Received(1).QueryGrid(
                Arg.Any<TestManagerQueryGridModel>(),
                Arg.Any<CommonPageModel>());
            Assert.NotNull(output);
            Assert.IsTrue(output.Success);

            output.Data.Data.ForEach(f => Console.WriteLine($"ROW:{ f.ROW }"));
        }
        [Test]
        public async Task Test_TestManager_QueryGrid()
        {
            var dapperService = Substitute.For<IDapperService<DefaultDbContext>>();
            dapperService.SqlQueryByPage<TestManagerQueryDto>(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>(),
                Arg.Any<CommonPageModel>())
            .Returns(Task.FromResult(new CommonQueryPageResultModel<TestManagerQueryDto>()
            {
                Data = new List<TestManagerQueryDto>(),
                Page = new CommonPageModel()
            }))
            .AndDoes(a =>
            {
                Utility.PrintSqlString(a.ArgAt<string>(0));
                Utility.PrintSqlString(a.ArgAt<string>(1));
                Utility.PrintDynamicParameters(a.ArgAt<DynamicParameters>(2));
            });

            var testManager = new TestManager(_loggerTestManager,
                _defaultDbContext, 
                dapperService, 
                _sqlUtil);
            var input = new TestManagerQueryGridModel()
            {
                NAME = "Mock"
            };
            var commonPageModel = new CommonPageModel()
            {
                PageNo = 3,
                PageSize = 5,
            };
            var output = await testManager.QueryGrid(input, commonPageModel);

            await dapperService.Received(1).SqlQueryByPage<TestManagerQueryDto>(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<DynamicParameters>(),
                Arg.Any<CommonPageModel>());
            Assert.NotNull(output);

            output.Data.ForEach(f => Console.WriteLine($"NAME:{ f.NAME }"));
        }
    }
}
