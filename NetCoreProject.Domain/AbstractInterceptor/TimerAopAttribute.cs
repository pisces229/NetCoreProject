using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.AbstractInterceptor
{
    public class TimerAopAttribute : AbstractInterceptorAttribute
    {
        [FromServiceContext]
        private ILogger<TimerAopAttribute> _logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var message = new StringBuilder();
            message.Append($"[{ context.Implementation }]");
            message.Append($"[{ context.ImplementationMethod.Name }]");
            var ticksStart = DateTime.Now.Ticks;
            try
            {
                await next(context);
            }
            finally
            {
                var ticksEnd = DateTime.Now.Ticks;
                message.Append($"[{ new TimeSpan(ticksEnd - ticksStart).TotalMilliseconds }]ms");
                _logger.LogInformation(message.ToString());
            }
        }
    }
}
