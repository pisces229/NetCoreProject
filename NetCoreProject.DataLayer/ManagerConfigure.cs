using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NetCoreProject.DataLayer.IManager;
using NetCoreProject.DataLayer.Manager;

namespace NetCoreProject.DataLayer
{
    public class ManagerConfigure
    {
        public static void AddService(IServiceCollection services)
        {
            services.AddScoped<IDefaultManager, DefaultManager>();
            services.AddScoped<ITestManager, TestManager>();
            services.AddScoped<ILoginManager, LoginManager>();
        }
    }
}
