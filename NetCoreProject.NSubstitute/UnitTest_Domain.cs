using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.Enum;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Util;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.NSubstitute
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
            {
                Console.WriteLine("---------- SqlQuery ----------");
                var dapperService = Substitute.For<IDapperService<DefaultDbContext>>();
                dapperService.SqlQuery<CommonOptionModel>(
                    Arg.Any<string>(),
                    Arg.Any<DynamicParameters>())
                .Returns(Task.FromResult(new List<CommonOptionModel>()));
                var result = await dapperService.SqlQuery<CommonOptionModel>("", new DynamicParameters());
                Assert.AreEqual(0, result.Count());
                Console.WriteLine("Received(1)");
                await dapperService.Received(1).SqlQuery<CommonOptionModel>(
                    Arg.Any<string>(),
                    Arg.Any<DynamicParameters>());
                dapperService.ClearReceivedCalls();
                Console.WriteLine("DidNotReceive");
                await dapperService.DidNotReceive().SqlQuery<CommonOptionModel>(
                    Arg.Any<string>(),
                    Arg.Any<DynamicParameters>());
            }
            {
                Console.WriteLine("---------- SqlQueryByPage ----------");
                var dapperService = Substitute.For<IDapperService<DefaultDbContext>>();
                dapperService.SqlQueryByPage<CommonOptionModel>(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<DynamicParameters>(),
                    Arg.Any<CommonPageModel>())
                .Returns(Task.FromResult(new CommonQueryPageResultModel<CommonOptionModel>()
                {
                    Data = new List<CommonOptionModel>(),
                    Page = new CommonPageModel()
                }));
                dapperService.When(w => w.SqlQueryByPage<CommonOptionModel>(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<DynamicParameters>(),
                    Arg.Any<CommonPageModel>()))
                .Do(d =>
                {
                    Console.WriteLine(d.ArgAt<string>(0));
                    Console.WriteLine(d.ArgAt<string>(1));
                    Console.WriteLine(d.ArgAt<DynamicParameters>(2));
                    Console.WriteLine(d.ArgAt<CommonPageModel>(3));
                });
                var result = await dapperService.SqlQueryByPage<CommonOptionModel>("select 1", "select 2", new DynamicParameters(), new CommonPageModel());
                Assert.NotNull(result);
            }
            {
                Console.WriteLine("---------- ExecuteScalar ----------");
                var dapperService = Substitute.For<IDapperService<DefaultDbContext>>();
                dapperService.ExecuteScalar<string>(
                    Arg.Any<string>())
                    //Arg.Is<string>(i => i == "select 2"))
                .Returns(
                    Task.FromResult("First"), 
                    Task.FromResult("Second"));
                dapperService.When(w => w.ExecuteScalar<string>(
                    Arg.Any<string>()))
                .Do(d =>
                {
                    Console.WriteLine(d.ArgAt<string>(0));
                });
                Assert.AreEqual("First", await dapperService.ExecuteScalar<string>("select 1"));
                Assert.AreEqual("Second", await dapperService.ExecuteScalar<string>("select 2"));
                //Assert.IsEmpty(await dapperService.ExecuteScalar<string>("select 1"));
                //Assert.AreEqual("First", await dapperService.ExecuteScalar<string>("select 2"));
                await dapperService.Received(2).ExecuteScalar<string>(Arg.Any<string>());
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