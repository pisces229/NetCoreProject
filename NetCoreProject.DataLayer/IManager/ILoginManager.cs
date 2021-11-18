using NetCoreProject.Domain.Model;
using System.Threading.Tasks;

namespace NetCoreProject.DataLayer.IManager
{
    public interface ILoginManager
    {
        Task<bool> ValidateUser(string model);
        Task<CommonTokenModel> GetToken(string token);
        Task SaveToken(string token, CommonTokenModel value);
        Task<bool> ValidateToken(string token);
        Task UpdateToken(string token, CommonTokenModel value);
        Task RemoveToken(string token);
    }
}
