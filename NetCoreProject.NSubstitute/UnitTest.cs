using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.NSubstitute
{
    public class UnitTest
    {
        public interface ICalculator
        {
            Task<int> Add(int a, int b);
        }
        [Test]
        public async Task Test1()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(Arg.Any<int>(), Arg.Do<int>(a => Console.WriteLine($"Arg.Do:{ a }")))
                .Returns(r => r.ArgAt<int>(0) + 1)
                //.ReturnsForAnyArgs(x => 0)
                .AndDoes(a =>
                {
                    Console.WriteLine($"AndDoes:{ a.ArgAt<int>(0) }");
                });

            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(1, 0) }");
            Console.WriteLine($">{ await calculator.Add(2, 0) }");
            Console.WriteLine($">{ await calculator.Add(1, 1) }");

            await calculator.Received(1).Add(0, 0);
            await calculator.Received(3).Add(Arg.Any<int>(), Arg.Is<int>(a => a == 0));
            await calculator.DidNotReceive().Add(3, 0);
        }
        [Test]
        public async Task Test2()
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

            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(1, 0) }");
            Console.WriteLine($">{ await calculator.Add(2, 0) }");
            Console.WriteLine($">{ await calculator.Add(3, 0) }");
            Console.WriteLine($">{ await calculator.Add(4, 0) }");


        }
        [Test]
        public async Task Test3()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(1, 2, 3, 4, 5);

            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(0, 0) }");

            await calculator.Received(5).Add(Arg.Any<int>(), Arg.Any<int>());
        }
        [Test]
        public async Task Test4()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(1, 2, 3, 4, 5);
                
            Console.WriteLine($">{ await calculator.Add(0, 0) }");
            Console.WriteLine($">{ await calculator.Add(1, 0) }");
            Console.WriteLine($">{ await calculator.Add(2, 0) }");

            Received.InOrder(() => {
                calculator.Add(Arg.Is<int>(i => i == 0), Arg.Is<int>(i => i == 0));
                calculator.Add(Arg.Is<int>(i => i == 1), Arg.Is<int>(i => i == 0));
                //calculator.Add(Arg.Is<int>(i => i == 1), Arg.Is<int>(i => i == 0));
                calculator.Add(Arg.Is<int>(i => i == 2), Arg.Is<int>(i => i == 0));
            });
        }
    }
}
