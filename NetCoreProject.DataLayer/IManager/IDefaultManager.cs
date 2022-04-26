using NetCoreProject.DataLayer.Model.Default;
using System.Threading.Tasks;

namespace NetCoreProject.DataLayer.IManager
{
    public interface IDefaultManager
    {
        Task<DefaultManagerRunDto> Run(DefaultManagerRunModel model);
    }
}
