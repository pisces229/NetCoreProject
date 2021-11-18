using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreProject.NSubstitute
{
    public class UnitTest
    {
        public interface ICalculator
        {
            int Add(int a, int b);
        }
        [Test]
        public void Test1()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(r => r.ArgAt<int>(0) + 1)
                //.ReturnsForAnyArgs(x => 0)
                .AndDoes(a =>
                {
                    Console.WriteLine($"AndDoes:{ a.ArgAt<int>(0) }");
                });
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(1, 0) }");
            Console.WriteLine($">{ calculator.Add(2, 0) }");
        }
        [Test]
        public void Test2()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.When(x => x.Add(Arg.Any<int>(), Arg.Any<int>()))
              .Do(
                Callback.First(x => Console.WriteLine("First"))
                    .Then(x => Console.WriteLine("Then:2"))
                    .Then(x => Console.WriteLine("Then:3"))
                    .ThenKeepDoing(x => Console.WriteLine("ThenKeepDoing:4"))
                    .AndAlways(x => Console.WriteLine("Always"))
              );
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(1, 0) }");
            Console.WriteLine($">{ calculator.Add(2, 0) }");
            Console.WriteLine($">{ calculator.Add(3, 0) }");
            Console.WriteLine($">{ calculator.Add(4, 0) }");
        }
        [Test]
        public void Test3()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(1, 2, 3, 4, 5);
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            calculator.Received(5).Add(Arg.Any<int>(), Arg.Any<int>());
        }
        [Test]
        public void Test4()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(1, 2, 3, 4, 5);
            Console.WriteLine($">{ calculator.Add(0, 0) }");
            Console.WriteLine($">{ calculator.Add(1, 0) }");
            Console.WriteLine($">{ calculator.Add(2, 0) }");
            Received.InOrder(() => {
                calculator.Add(Arg.Is<int>(i => i == 0), Arg.Is<int>(i => i == 0));
                calculator.Add(Arg.Is<int>(i => i == 1), Arg.Is<int>(i => i == 0));
                //calculator.Add(Arg.Is<int>(i => i == 1), Arg.Is<int>(i => i == 0));
                calculator.Add(Arg.Is<int>(i => i == 2), Arg.Is<int>(i => i == 0));
            });
        }
    }
}
