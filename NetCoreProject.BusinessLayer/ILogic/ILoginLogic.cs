using NetCoreProject.Domain.Model;
using NetCoreProject.BusinessLayer.Model.Login;
using System.Threading.Tasks;

namespace NetCoreProject.BusinessLayer.ILogic
{
    public interface ILoginLogic
    {
        Task<CommonApiResultModel<string>> SignIn(LoginSignInInputModel model);
        Task SignOut(string model);
        Task<string> Refresh(string model);
    }
}
