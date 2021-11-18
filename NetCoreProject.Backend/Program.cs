using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace NetCoreProject.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            ////1. Get the IWebHost which will host this application.
            //var host = CreateHostBuilder(args).Build();
            ////2. Find the service layer within our scope.
            //using (var scope = host.Services.CreateScope())
            //{
            //    //3. Get the instance of BoardGamesDBContext in our services layer
            //    var services = scope.ServiceProvider;
            //    var context = services.GetRequiredService<DefaultDbContext>();
            //    //4. Call the DataGenerator to create sample data
            //    DefaultDataGenerator.Initialize(services);
            //}
            ////Continue to run the application
            //host.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseNLog();
                })
                .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());

    }
}
