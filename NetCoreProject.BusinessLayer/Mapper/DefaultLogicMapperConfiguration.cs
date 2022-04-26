using NetCoreProject.DataLayer.Model.Default;
using NetCoreProject.BusinessLayer.Model.Default;
using AutoMapper;

namespace NetCoreProject.BusinessLayer.Mapper
{
    public class DefaultLogicMapperConfiguration
    {
        public static void CreateMap(IMapperConfigurationExpression mapperConfiguration)
        {
            mapperConfiguration.CreateMap<DefaultLogicRunInputModel, DefaultManagerRunModel>();
            mapperConfiguration.CreateMap<DefaultManagerRunDto, DefaultLogicRunOutputModel>();
        }
    }
}
