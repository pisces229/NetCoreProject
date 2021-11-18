using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NetCoreProject.BusinessLayer;
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
        private readonly Mock<IDefaultDataProtector> _defaultDataProtector;
        private readonly DefaultDbContext _defaultDbContext;
        private readonly IMapper _mapper;
        private readonly SqlUtil _sqlUtil;
        private readonly ConfigurationUtil _configurationUtil;
        public UnitTest_Test()
        {
            _defaultDataProtector = new Mock<IDefaultDataProtector>();
            _defaultDataProtector.Setup(s => s.Protect(It.IsAny<string>())).Returns<string>(r => r);
            _defaultDataProtector.Setup(s => s.Unprotect(It.IsAny<string>())).Returns<string>(r => r);
            var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "DefaultDbContext");
            _defaultDbContext = new DefaultDbContext(optionsBuilder.Options);
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    {"Temp", "Temp"}
                })
                .Build();
            _mapper = new Mapper(new MapperConfiguration(c => LogicConfigure.CreateMap(c)));
            _sqlUtil = new SqlUtil();
            _configurationUtil = new ConfigurationUtil(configuration);
        }
        [SetUp]
        public void Setup()
        {
            
        }
        [Test]
        public async Task Test_TestManager_QueryGrid()
        {
            var mockLogger = new Mock<ILogger<TestManager>>();
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

            var testManager = new TestManager(mockLogger.Object, 
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
        }
        [Test]
        public async Task Test_TestLogic_QueryGrid()
        {
            var mockLogger = new Mock<ILogger<TestLogic>>();
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

            var testLogic = new TestLogic(mockLogger.Object, 
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

            Assert.NotNull(output);
            mockTestManager.Verify(v => v.QueryGrid(
                It.IsAny<TestManagerQueryGridModel>(),
                It.IsAny<CommonPageModel>())
            , Times.Once);
            if (!output.Success)
            {
                Assert.Fail();
            }
            else
            {
                output.Data.Data.ForEach(f => Console.WriteLine($"ROW:{ f.ROW }"));
            }
        }
    }
}
