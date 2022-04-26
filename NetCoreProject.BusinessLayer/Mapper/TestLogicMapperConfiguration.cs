using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.DataLayer.Model.Test;
using NetCoreProject.Domain.Entity;
using NetCoreProject.Domain.Model;
using AutoMapper;

namespace NetCoreProject.BusinessLayer.Mapper
{
    public class TestLogicMapperConfiguration
    {
        public static void CreateMap(IMapperConfigurationExpression mapperConfiguration)
        {
            // Mapping different Name 
            // .ForMember(dest => dest.NAME, opt => opt.MapFrom(src => src.NAME));
            mapperConfiguration.CreateMap<Test, TestLogicOutputModel>();
            mapperConfiguration.CreateMap<TestLogicInputModel, TestManagerQueryModel>();
            mapperConfiguration.CreateMap<TestManagerQueryDto, TestLogicOutputModel>();
            mapperConfiguration.CreateMap<TestLogicQueryGridInputModel, TestManagerQueryGridModel>();
            mapperConfiguration.CreateMap<TestManagerQueryDto, TestLogicQueryGridOutputModel>();
        }
    }
}
