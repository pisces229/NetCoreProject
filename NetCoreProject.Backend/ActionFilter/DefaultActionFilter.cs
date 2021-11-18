using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.ActionFilter
{
    public class DefaultActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<DefaultActionFilter> _logger;
        public DefaultActionFilter(ILogger<DefaultActionFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("DefaultActionFilter Before");
            //await context.HttpContext.Request.ReadFormAsync();
            await next();
            //await context.HttpContext.Response.WriteAsync("...");
            _logger.LogInformation("DefaultActionFilter After");
        }
    }
}
