using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.Middleware
{
    public class DefaultMiddleware
    {
        private readonly ILogger<DefaultMiddleware> _logger;
        private readonly RequestDelegate _dequestDelegate;
        public DefaultMiddleware(ILogger<DefaultMiddleware> logger, 
            RequestDelegate requestDelegate)
        {
            _logger = logger;
            _dequestDelegate = requestDelegate;
        }
        public async Task Invoke(HttpContext context)
        {
            var correlationGuid = Guid.NewGuid().ToString();
            context.Request.Headers.Add("CorrelationGuid", correlationGuid);
            _logger.LogInformation(context.Request.Path);
            await _dequestDelegate(context);
            context.Response.Headers.Add("CorrelationGuid", correlationGuid);
        }
    }
}
