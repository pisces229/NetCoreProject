using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.Csp
{
    public class CspMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CspOptions _options;
        public CspMiddleware(RequestDelegate next, CspOptions options)
        {
            _next = next;
            _options = options;
        }
        private string Header => _options.ReadOnly ? "Content-Security-Policy-Report-Only" : "Content-Security-Policy";
        private string HeaderValue
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(_options.DefaultSrc);
                stringBuilder.Append(_options.ConnectSrc);
                stringBuilder.Append(_options.FontSrc);
                stringBuilder.Append(_options.ChildSrc);
                stringBuilder.Append(_options.FrameSrc);
                stringBuilder.Append(_options.ImageSrc);
                stringBuilder.Append(_options.MediaSrc);
                stringBuilder.Append(_options.ObjectSrc);
                stringBuilder.Append(_options.ScriptSrc);
                stringBuilder.Append(_options.StyleSrc);
                stringBuilder.Append(_options.FrameAncestors);
                if (!string.IsNullOrEmpty(_options.ReportURL))
                {
                    stringBuilder.Append($"report-uri {_options.ReportURL};");
                }
                return stringBuilder.ToString();
            }
        }
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add(Header, HeaderValue);
            await _next(context);
        }
    }
}
