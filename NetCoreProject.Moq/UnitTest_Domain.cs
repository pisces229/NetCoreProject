using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.Enum;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.Moq
{
    public class UnitTest_Domain
    {
        [SetUp]
        public void Setup()
        {
            
        }
        [Test]
        public async Task Test_DbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "DefaultDbContext");
            var defaultDbContext = new DefaultDbContext(optionsBuilder.Options);
            {
                Console.WriteLine("---------- Add ----------");
                var data = new Domain.Entity.Test()
                {
                    NAME = "Mock",
                    MAKE_DATE = DateTime.Now,
                    REMARK = "test"
                };
                defaultDbContext.Test.Add(data);
                await defaultDbContext.SaveChangesAsync();
                Assert.AreEqual(1, await defaultDbContext.Test.CountAsync());
                defaultDbContext.Entry(data).State = EntityState.Detached;
            }
            {
                Console.WriteLine("---------- Update ----------");
                var data = await defaultDbContext.Test.FirstOrDefaultAsync();
                Assert.AreEqual("Mock", data.NAME);
                data.NAME = "MockModify";
                defaultDbContext.Test.Update(data);
                await defaultDbContext.SaveChangesAsync();
                Assert.AreEqual("MockModify", (await defaultDbContext.Test.FirstOrDefaultAsync()).NAME);
                defaultDbContext.Entry(data).State = EntityState.Detached;
            }
            {
                Console.WriteLine("---------- Remove ----------");
                var data = await defaultDbContext.Test.FirstOrDefaultAsync();
                defaultDbContext.Test.Remove(data);
                await defaultDbContext.SaveChangesAsync();
                Assert.AreEqual(false, await defaultDbContext.Test.AnyAsync());
                defaultDbContext.Entry(data).State = EntityState.Detached;
            }
            {
                Console.WriteLine("---------- Transaction ----------");
                await defaultDbContext.BeginTransaction();
                defaultDbContext.Commit();
                defaultDbContext.Rollback();
            }
        }
        [Test]
        public async Task Test_DapperService()
        {
            var mockDapperService = new Mock<IDapperService<DefaultDbContext>>();
            {
                Console.WriteLine("---------- SqlQuery ----------");
                mockDapperService.Setup(s => s.SqlQuery<CommonOptionModel>(
                        It.IsAny<string>(), 
                        It.IsAny<DynamicParameters>(), 
                        It.IsAny<int?>(), 
                        It.IsAny<CommandType>()))
                    .Callback(() => Console.WriteLine("Before"))
                    .Returns(Task.FromResult(new List<CommonOptionModel>()))
                    .Callback(() => Console.WriteLine("After"));
                var result = await mockDapperService.Object.SqlQuery<CommonOptionModel>("", new DynamicParameters());
                Assert.AreEqual(0, result.Count());
                mockDapperService.Verify(v => v.SqlQuery<CommonOptionModel>(
                        It.IsAny<string>(), 
                        It.IsAny<DynamicParameters>(), 
                        It.IsAny<int?>(), 
                        It.IsAny<CommandType>()),
                    Times.Once);
            }
            {
                Console.WriteLine("---------- SqlQueryByPage ----------");
                mockDapperService.Setup(s => s.SqlQueryByPage<CommonOptionModel>(
                        It.IsAny<string>(), 
                        It.IsAny<string>(), 
                        It.IsAny<DynamicParameters>(), 
                        It.IsAny<CommonPageModel>(), 
                        It.IsAny<int?>(), 
                        It.IsAny<CommandType>()))
                    .Callback(() => Console.WriteLine("Before"))
                    .Returns(Task.FromResult(new CommonQueryPageResultModel<CommonOptionModel>()
                    { 
                        Data = new List<CommonOptionModel>(),
                        Page = new CommonPageModel()
                    }))
                    .Callback(() => Console.WriteLine("After"));
                var result = await mockDapperService.Object.SqlQueryByPage<CommonOptionModel>("", "", new DynamicParameters(), new CommonPageModel());
                Assert.NotNull(result);
                mockDapperService.Verify(v => v.SqlQueryByPage<CommonOptionModel>(
                        It.IsAny<string>(), 
                        It.IsAny<string>(), 
                        It.IsAny<DynamicParameters>(), 
                        It.IsAny<CommonPageModel>(), 
                        It.IsAny<int?>(), 
                        It.IsAny<CommandType>()),
                    Times.Once);
            }
            {
                Console.WriteLine("---------- ExecuteScalar ----------");
                mockDapperService.SetupSequence(s => s.ExecuteScalar<string>(
                        It.IsAny<string>(), 
                        It.IsAny<DynamicParameters>(), 
                        It.IsAny<int?>(), 
                        It.IsAny<CommandType>()))
                    .Returns(Task.FromResult("First"))
                    .Returns(Task.FromResult("Second"))
                    //.Returns(Task.FromResult("Third"))
                    .Throws(new InvalidOperationException("Third"));
                Assert.AreEqual("First", await mockDapperService.Object.ExecuteScalar<string>("", new DynamicParameters()));
                Assert.AreEqual("Second", await mockDapperService.Object.ExecuteScalar<string>("", new DynamicParameters()));
                //Assert.AreEqual("Third", await mockDapperService.Object.ExecuteScalar<string>("", new DynamicParameters()));
            }
        }
        [Test]
        public void Test_SqlUtil()
        {
            var sqlUtil = new SqlUtil();
            var condition = new StringBuilder();
            var dynamicParameters = new DynamicParameters();
            sqlUtil.WhereAndCondition(condition, dynamicParameters, "A", OperatorEnum.EQUAL, "B");
            Utility.PrintSqlString(condition.ToString());
            Utility.PrintDynamicParameters(dynamicParameters);
        }
        [Test]
        public void Test_ConfigurationUtil()
        {
            {
                var configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(
                    new Dictionary<string, string> {
                        { "TempPath", "Test" }
                    }).Build();
                var configurationUtil = new ConfigurationUtil(configurationRoot);
                Assert.AreEqual("Test", configurationUtil.TempPath);
            }
            {
                var configurationRoot = Utility.CreateConfiguration();
                var configurationUtil = new ConfigurationUtil(configurationRoot);
                Assert.AreEqual("d:/WorkSpace/NetCoreProject/Temp", configurationUtil.TempPath);
            }
        }
    }
}