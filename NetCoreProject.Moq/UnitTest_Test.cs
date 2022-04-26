using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace NetCoreProject.Moq
{
    public class UnitTest_Test
    {
        private readonly ILogger<TestController> _loggerTestController;
        private readonly ILogger<TestLogic> _loggerTestLogic;
        private readonly ILogger<TestManager> _loggerTestManager;
        private readonly DefaultDbContext _defaultDbContext;
        private readonly Mock<IDefaultDataProtector> _defaultDataProtector;
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
            _defaultDataProtector = new Mock<IDefaultDataProtector>();
            _defaultDataProtector.Setup(s => s.Protect(It.IsAny<string>())).Returns<string>(r => r);
            _defaultDataProtector.Setup(s => s.Unprotect(It.IsAny<string>())).Returns<string>(r => r);
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
            var testLogic = new Mock<ITestLogic>();
            testLogic.Setup(s => s.QueryGrid(
                It.IsAny<CommonQueryPageModel<TestLogicQueryGridInputModel>>()))
            .ReturnsAsync(new CommonApiResultModel<CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>>()
            {
                Success = true,
                Data = new CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>()
            });

            var testController = new TestController(_loggerTestController,
                testLogic.Object,
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

            testLogic.Verify(v => v.QueryGrid(
                It.IsAny<CommonQueryPageModel<TestLogicQueryGridInputModel>>())
            , Times.Once);
            Assert.NotNull(output);

            Console.WriteLine(output);
        }
        [Test]
        public async Task Test_TestLogic_QueryGrid()
        {
            var mockTestManager = new Mock<ITestManager>();
            mockTestManager.Setup(s => s.QueryGrid(
                    It.IsAny<TestManagerQueryGridModel>(),
                    It.IsAny<CommonPageModel>()))
                .ReturnsAsync(new CommonQueryPageResultModel<TestManagerQueryDto>()
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
                })
                .Callback<TestManagerQueryGridModel, CommonPageModel>((c1, c2) =>
                {
                    Console.WriteLine($"NAME:{ c1.NAME }");
                });

            var testLogic = new TestLogic(_loggerTestLogic,
                _defaultDataProtector.Object,
                _defaultDbContext,
                _mapper,
                mockTestManager.Object,
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

            mockTestManager.Verify(v => v.QueryGrid(
                It.IsAny<TestManagerQueryGridModel>(),
                It.IsAny<CommonPageModel>())
            , Times.Once);
            Assert.NotNull(output);
            Assert.IsTrue(output.Success);

            output.Data.Data.ForEach(f => Console.WriteLine($"ROW:{ f.ROW }"));
        }
        [Test]
        public async Task Test_TestManager_QueryGrid()
        {
            var mockDapperService = new Mock<IDapperService<DefaultDbContext>>();
            mockDapperService.Setup(s => s.SqlQueryByPage<TestManagerQueryDto>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    It.IsAny<CommonPageModel>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>()))
                .ReturnsAsync(new CommonQueryPageResultModel<TestManagerQueryDto>()
                {
                    Data = new List<TestManagerQueryDto>(),
                    Page = new CommonPageModel()
                })
                .Callback<string, string, DynamicParameters, CommonPageModel, int?, CommandType>((c1, c2, c3, c4, c5, c6) =>
                {
                    Utility.PrintSqlString(c1);
                    Utility.PrintSqlString(c2);
                    Utility.PrintDynamicParameters(c3);
                });

            var testManager = new TestManager(_loggerTestManager, 
                _defaultDbContext, 
                mockDapperService.Object, 
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

            Assert.NotNull(output);
            mockDapperService.Verify(v => v.SqlQueryByPage<TestManagerQueryDto>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    It.IsAny<CommonPageModel>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>())
                , Times.Once);

            output.Data.ForEach(f => Console.WriteLine($"NAME:{ f.NAME }"));
        }
    }
}
