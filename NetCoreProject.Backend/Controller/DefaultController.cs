using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Model.Default;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
        private readonly IDefaultLogic _defaultLogic;
        public DefaultController(ILogger<DefaultController> logger,
            IDefaultLogic defaultLogic)
        {
            _logger = logger;
            _defaultLogic = defaultLogic;
        }
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            return Ok(await Task.FromResult("Test"));
        }
        [HttpPost]
        public async Task<ActionResult> Run([FromBody] DefaultLogicRunInputModel model)
        {
            var result = await _defaultLogic.Run(model);
            return Ok(result);
        }
    }
}