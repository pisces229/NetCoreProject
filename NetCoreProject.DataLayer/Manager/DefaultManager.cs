using NetCoreProject.DataLayer.IManager;
using NetCoreProject.DataLayer.Model.Default;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NetCoreProject.DataLayer.Manager
{
    public class DefaultManager : IDefaultManager
    {
        private readonly ILogger<DefaultManager> _logger;
        public DefaultManager(ILogger<DefaultManager> logger)
        {
            _logger = logger;;
        }
        public async Task<DefaultManagerRunDto> Run(DefaultManagerRunModel model)
        {
            return await Task.FromResult(new DefaultManagerRunDto()
            {
                Result = "Success"
            });
        }
    }
}
