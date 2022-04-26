using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.AbstractInterceptor
{
    public class InputAopAttribute : AbstractInterceptorAttribute
    {
        [FromServiceContext]
        private ILogger<InputAopAttribute> _logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var message = new StringBuilder();
            message.Append($"[{ context.Implementation }]");
            message.Append($"[{ context.ImplementationMethod.Name }]");
            context.Parameters.ToList().ForEach(f =>
            {
                if (f != null)
                {
                    message.Append(JsonSerializer.Serialize(f));
                }
            });
            _logger.LogInformation(message.ToString());
            await next(context);
        }
    }
}
