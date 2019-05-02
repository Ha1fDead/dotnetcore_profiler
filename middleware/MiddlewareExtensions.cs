using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace middleware
{
    public static class MiddlewareExtensions
    {
        /// Middleware injects profiled data into rendered `text/html` pages and updates content length accordingly
        /// Optional Middleware
        public static IApplicationBuilder UseHtmlInsertMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HtmlInsertMiddleware>();
        }

        /// Profiles requests for execution time and generated content length
        public static IApplicationBuilder UseRequestProfilerMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestProfilerMiddleware>();
        }
    }
}
