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
    public class UnitTest
    {
        public interface ICalculator
        {
            Task<int> Add(int a, int b);
        }
        [SetUp]
        public void Setup()
        {
            
        }
        [Test]
        public async Task Test()
        {
            var calculator = new Mock<ICalculator>();
            calculator.Setup(s => s.Add(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((a, b) => Console.WriteLine($"Before a:[{ a }] b:[{ b }]"))
                .ReturnsAsync(0)
                .Callback<int, int>((a, b) => Console.WriteLine($"After a:[{ a }] b:[{ b }]"));

            var result = await calculator.Object.Add(0, 0);

            calculator.Verify(v => v.Add(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual(0, result);

            Console.WriteLine(result);
        }
    }
}