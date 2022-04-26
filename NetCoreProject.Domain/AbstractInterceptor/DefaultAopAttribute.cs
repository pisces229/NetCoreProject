using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.AbstractInterceptor
{
    public class DefaultAopAttribute : AbstractInterceptorAttribute
    {
        [FromServiceContext]
        private ILogger<DefaultAopAttribute> _logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }
    }
}
