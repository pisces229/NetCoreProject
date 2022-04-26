using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.ResourceFilter
{
    public class DefaultResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<DefaultResourceFilter> _logger;
        public DefaultResourceFilter(ILogger<DefaultResourceFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("DefaultResourceFilter Before");
            //await context.HttpContext.Request.ReadFormAsync();
            await next();
            //await context.HttpContext.Response.WriteAsync("...");
            _logger.LogInformation("DefaultResourceFilter After");
        }
    }
}
