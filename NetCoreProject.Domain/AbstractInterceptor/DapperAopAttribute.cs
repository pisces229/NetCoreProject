using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.AbstractInterceptor
{
    public class DapperAopAttribute : AbstractInterceptorAttribute
    {
        private readonly Regex _regex = new Regex("[ ]{2,}", RegexOptions.None);
        [FromServiceContext]
        private ILogger<DapperAopAttribute> _logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var message = new StringBuilder();
            message.AppendLine(context.Implementation.ToString());
            if (!"SqlQueryByPage".Equals(context.ImplementationMethod.Name))
            {
                GetSqlString(message, context.Parameters.FirstOrDefault());
                GetDynamicParameters(message, context.Parameters.Skip(1).FirstOrDefault());
            }
            else
            {
                GetSqlString(message, context.Parameters.FirstOrDefault());
                GetSqlString(message, context.Parameters.Skip(1).FirstOrDefault());
                GetDynamicParameters(message, context.Parameters.Skip(2).FirstOrDefault());
            }
            var ticksStart = DateTime.Now.Ticks;
            try
            {
                await next(context);
                var ticksEnd = DateTime.Now.Ticks;
                message.Append($"-- time:[{ new TimeSpan(ticksEnd - ticksStart).TotalMilliseconds }]ms");
                _logger.LogInformation(message.ToString());
            }
            catch
            {
                var ticksEnd = DateTime.Now.Ticks;
                message.AppendLine($"-- time:[{ new TimeSpan(ticksEnd - ticksStart).TotalMilliseconds }]ms");
                _logger.LogError(message.ToString());
                throw;
            }
        }
        private void GetSqlString(StringBuilder message, object parameter)
        {
            if (parameter != null && typeof(string) == parameter.GetType())
            {
                message.AppendLine(_regex.Replace(parameter.ToString().Replace(Environment.NewLine, " "), " "));
            }
        }
        private void GetDynamicParameters(StringBuilder message, object parameter)
        {
            if (parameter != null && typeof(DynamicParameters) == parameter.GetType())
            {
                var dynamicParameters = (DynamicParameters)parameter;
                var parameterNames = ((DynamicParameters)parameter).ParameterNames.ToList();
                if (parameterNames.Any())
                {
                    message.Append("-- parameters:");
                    parameterNames.ForEach(name =>
                    {
                        var dynamicParameter = dynamicParameters.Get<object>(name);
                        if (dynamicParameter != null)
                        {
                            var dynamicParameterType = dynamicParameter.GetType();
                            if (dynamicParameter is Array)
                            {
                                message.Append($"[{ name }]:[{ GetArrayString(dynamicParameter as Array) }],");
                            }
                            else if (dynamicParameter is IList)
                            {
                                message.Append($"[{ name }]:[{ GetListString(dynamicParameter as IList) }],");
                            }
                            else
                            {
                                message.Append($"[{ name }]:[{ dynamicParameter }],");
                            }
                        }
                        else
                        {
                            message.Append($"[{ name }]:[NULL],");
                        }
                    });
                }
            }
        }
        private string GetArrayString(Array values)
        {
            var result = new StringBuilder();
            foreach (var value in values)
            {
                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append(value.ToString());
            }
            return result.ToString();
        }
        private string GetListString(IList values)
        {
            var result = new StringBuilder();
            foreach (var value in values)
            {
                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append(value != null ? value.ToString() : "null");
            }
            return result.ToString();
        }
    }
}
