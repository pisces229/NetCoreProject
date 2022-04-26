using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Logic;
using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.DataLayer.Manager;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace NetCoreProject.NUnit
{
    public class NUnitTest_Test : NUnitTest
    {
        private readonly ILogger<TestLogic> _loggerTestLogic;
        private readonly ILogger<TestManager> _loggerTestManager;
        private readonly ITestLogic _testLogic;
        public NUnitTest_Test() : base()
        {
            var service = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .BuildServiceProvider();
            _loggerTestLogic = service.GetService<ILoggerFactory>().CreateLogger<TestLogic>();
            _loggerTestManager = service.GetService<ILoggerFactory>().CreateLogger<TestManager>();
            _testLogic = GetHost().Services.GetRequiredService<ITestLogic>();
        }
        [SetUp]
        public async Task Setup()
        {
            await Task.FromResult("do something");
        }
        [Test]
        public async Task Test_TestLogic_Insert()
        {
            var input = new TestLogicInputModel()
            {
                NAME = "Test",
                MAKE_DATE = DateTime.Now.AddDays(10),
                REMARK = "REMARK"
            };
            var output = await _testLogic.Insert(input);
            if (output.Success)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
