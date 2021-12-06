using Microsoft.Extensions.ObjectPool;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.NUnit
{
    public class TestSomething
    {
        [SetUp]
        public async Task Setup()
        {
            await Task.FromResult("do something");
        }
        [Test]
        public async Task Test()
        {
            await Task.FromResult("do something");
        }
    }
}
