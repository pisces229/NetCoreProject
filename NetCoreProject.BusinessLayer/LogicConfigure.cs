using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Logic;
using NetCoreProject.BusinessLayer.Mapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreProject.BusinessLayer
{
    public class LogicConfigure
    {
        public static void AddService(IServiceCollection services)
        {
            services.AddScoped<IDefaultLogic, DefaultLogic>();
            services.AddScoped<ITestLogic, TestLogic>();
            services.AddScoped<ILoginLogic, LoginLogic>();
        }
        public static void CreateMap(IMapperConfigurationExpression configure)
        {
            DefaultLogicMapperConfiguration.CreateMap(configure);
            TestLogicMapperConfiguration.CreateMap(configure);
        }
    }
}
