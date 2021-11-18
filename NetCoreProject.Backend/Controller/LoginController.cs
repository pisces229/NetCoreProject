using NetCoreProject.BusinessLayer.Model.Login;
using NetCoreProject.BusinessLayer.ILogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ILoginLogic _loginLogic;
        public LoginController(ILogger<LoginController> logger,
            ILoginLogic loginLogic)
        {
            _logger = logger;
            _loginLogic = loginLogic;
        }
        [HttpPost]
        public async Task<ActionResult> SignIn([FromBody] LoginSignInInputModel model)
        {
            return Ok(await _loginLogic.SignIn(model));
        }
        [HttpPost]
        public async Task<ActionResult> Refresh([FromBody] string model)
        {
            var result = await _loginLogic.Refresh(model);
            if (!string.IsNullOrEmpty(result))
            {
                return Ok(result);
            }
            else
            {
                return Forbid();
            }
        }
        [HttpPost]
        public async Task<ActionResult> SignOut([FromBody] string model)
        {
            await _loginLogic.SignOut(model);
            return Ok();
        }
    }
}
