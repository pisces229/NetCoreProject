using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.ResultFilter
{
    public class DefaultResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<DefaultResultFilter> _logger;
        public DefaultResultFilter(ILogger<DefaultResultFilter> logger)
        {
            _logger = logger;
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("DefaultResultFilter Before");
            //await context.HttpContext.Request.ReadFormAsync();
            await next();
            if (!context.ModelState.IsValid)
            {
                _logger.LogError("model validation errors occurred.");
            }
            //await context.HttpContext.Response.WriteAsync("...");
            _logger.LogInformation("DefaultResultFilter After");
        }
    }
}
