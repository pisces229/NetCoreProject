using System;
using System.Threading.Tasks;

namespace NetCoreProject.Batch
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Batch Start");
                Task.Run(async () =>
                {
                    var startup = new Startup();
                    await startup.Excute();
                }).Wait();
                Console.WriteLine("Batch End");
            }
            //catch (Exception e)
            //{
            //    Console.WriteLine("Batch catch");
            //    Console.WriteLine(e.ToString());
            //    Console.ReadLine();
            //}
            finally
            {
                Console.WriteLine("Batch Finally");
            }
        }
    }
}
