using AutoMapper;
using NetCoreProject.Batch.Model.Test;

namespace NetCoreProject.Batch.Mapper
{
    public class TestRunnerMapperConfiguration
    {
        public static void CreateMap(IMapperConfigurationExpression mapperConfiguration)
        {
            // Mapping different Name 
            // .ForMember(dest => dest.NAME, opt => opt.MapFrom(src => src.NAME));
            mapperConfiguration.CreateMap<TestRunnerMapperName1, TestRunnerMapperName2>();
            mapperConfiguration.CreateMap<TestRunnerMapperName1, TestRunnerMapperName3>();
            mapperConfiguration.CreateMap<TestRunnerMapperPerson1, TestRunnerMapperPerson2>();
            mapperConfiguration.CreateMap<TestRunnerMapperPerson1, TestRunnerMapperPerson3>();
        }
    }
}
