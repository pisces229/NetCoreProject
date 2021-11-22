using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetCoreProject.Domain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.AuthorizationFilter
{
    public class ValueAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _values;
        private readonly ILogger<ValueAuthorizationFilter> _logger;
        private readonly ConfigurationUtil _configurationUtil;
        public ValueAuthorizationFilter(string[] values,
            ILogger<ValueAuthorizationFilter> logger,
            ConfigurationUtil configurationUtil)
        {
            _values = values;
            _logger = logger;
            _configurationUtil = configurationUtil;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await Task.Run(() => _logger.LogInformation("ValueAuthorizationFilter"));
            _values.ToList().ForEach(f => _logger.LogInformation(f));
        }
    }
}
