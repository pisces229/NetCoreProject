
namespace NetCoreProject.Domain.IService
{
    public interface IDefaultDataProtector
    {
        public string Protect(string value);
        public string Unprotect(string value);
    }
}
