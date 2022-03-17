using Microsoft.AspNetCore.Builder;
using System;

namespace NetCoreProject.Backend.Csp
{
    public static class CspMiddlewareExtensions
    {
        public static IApplicationBuilder UseCsp(this IApplicationBuilder app, CspOptions options)
        {
            return app.UseMiddleware<CspMiddleware>(options);
        }
        public static IApplicationBuilder UseCsp(this IApplicationBuilder app, Action<CspOptions> optionsDelegate)
        {
            var options = new CspOptions();
            optionsDelegate(options);
            return app.UseMiddleware<CspMiddleware>(options);
        }
    }
}
