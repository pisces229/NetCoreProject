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

            RunPool();

            RunFuncAction();
        }
        #region RunPool
        private void RunPool()
        {
            Console.WriteLine("---------- RunPool ----------");
            var defalutPolicy = new DefaultPooledObjectPolicy<PersonInformation>();
            var defaultPool = new DefaultObjectPool<PersonInformation>(defalutPolicy, 1);
            for (int i = 0; i < 10; i++)
            {
                var pooledObject1 = defaultPool.Get();
                Console.WriteLine($"#{ pooledObject1.GetHashCode() }");
                var pooledObject2 = defaultPool.Get();
                Console.WriteLine($"#{ pooledObject2.GetHashCode() }");
                defaultPool.Return(pooledObject1);
                defaultPool.Return(pooledObject2);
            }
        }
        #endregion

        #region Func / Action
        private void RunFuncAction()
        {
            Console.WriteLine("---------- RunFuncAction ----------");
            Func<int, int, int> testFunc = (x, y) => (x * y);
            //Console.WriteLine(testFunc(3, 4));
            // Action
            var showAgeAction = new Action<PersonInformation>(ShowAgeAction);
            Action<PersonInformation> showNameAction = ShowAgeAction;

            var personInformation = new PersonInformation()
            {
                Age = 30,
                Name = "Joe"
            };

            //showAgeAction(actionData);
            //showNameAction(actionData);
            ActionExcute(showAgeAction, showNameAction, personInformation);

            var personInformations = new List<PersonInformation>()
            {
                new PersonInformation()
                {
                    Age = 12, Name = "A"
                },
                new PersonInformation()
                {
                    Age = 13, Name = "B"
                },
                new PersonInformation()
                {
                    Age = 14, Name = "C"
                },
            };

            Func<PersonInformation, int, bool> FilterAgeAction = (data, age) => data.Age > age;
            personInformations.ForEach(f =>
            {
                if (FilterAgeAction(f, 12))
                {
                    Console.WriteLine(f.Name);
                }
            });
        }
        private void ActionExcute<T>(Action<T> showAgeAction, Action<T> showNameAction, T actionData) where T : class
        {
            showAgeAction(actionData);
            showNameAction(actionData);
        }
        private void ShowAgeAction(PersonInformation data)
        {
            Console.WriteLine(data.Age);
        }
        private class PersonInformation
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }
}
