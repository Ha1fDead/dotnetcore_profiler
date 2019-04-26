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
        public static IApplicationBuilder UseHtmlInsertMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HtmlInsertMiddleware>();
        }

        public static IApplicationBuilder UseRequestDataCollectorMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestDataCollectorMiddleware>();
        }
    }
}
