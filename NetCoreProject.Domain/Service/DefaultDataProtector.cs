using Microsoft.AspNetCore.DataProtection;
using NetCoreProject.Domain.IService;

namespace NetCoreProject.Domain.Service
{
    public class DefaultDataProtector : IDefaultDataProtector
    {
        private readonly IDataProtector _dataProtector;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        public DefaultDataProtector(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _dataProtector = dataProtectionProvider.CreateProtector("test");
        }
        public string Protect(string value) => _dataProtector.Protect(value);
        public string Unprotect(string value) => _dataProtector.Unprotect(value);
    }
}
