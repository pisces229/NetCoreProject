using NetCoreProject.Domain.Model;
using NetCoreProject.BusinessLayer.Model.Default;
using System.Threading.Tasks;

namespace NetCoreProject.BusinessLayer.ILogic
{
    public interface IDefaultLogic
    {
        Task<CommonApiResultModel<DefaultLogicRunOutputModel>> Run(DefaultLogicRunInputModel model);
    }
}
