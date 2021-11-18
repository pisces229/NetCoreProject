using Microsoft.Extensions.Configuration;

namespace NetCoreProject.Domain.Util
{
    public class ConfigurationUtil
    {
        private readonly IConfiguration _configuration;
        public ConfigurationUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string TempPath => _configuration.GetValue<string>("TempPath");
    }
}
