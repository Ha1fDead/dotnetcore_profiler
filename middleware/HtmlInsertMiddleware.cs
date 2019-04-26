using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace middleware
{
    /// Writes request metadata to the request itself and returns
    public class HtmlInsertMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public HtmlInsertMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        // What about transfer encoding?
        public async Task InvokeAsync(HttpContext context)
        {
            // Due to middleware architecture, we don't have to worry about this stream having anything in it yet
            // This assumption won't be true after calling the request delegate
            var capturedBody = context.Response.Body;
            
            try
            {
                using (var swapBody = new MemoryStream())
                {
                    context.Response.Body = swapBody;
                    await _requestDelegate.Invoke(context);

                    if (RequestProfilerMiddleware.ProfiledRequests.Any())
                    {
                        var injected = this.GetInjected();
                        await swapBody.WriteAsync(injected, 0, injected.Length);
                        context.Response.ContentLength = swapBody.Length + injected.Length;
                    }

                    swapBody.Seek(0, SeekOrigin.Begin);
                    await swapBody.CopyToAsync(capturedBody);
                }
            }
            finally
            {
                context.Response.Body = capturedBody;
            }
        }

        private Byte[] GetInjected() {
            var shortest = RequestProfilerMiddleware.ProfiledRequests.Min((data) => {
                return data.RequestBodyLengthBytes;
            });
            var longest = RequestProfilerMiddleware.ProfiledRequests.Max((data) => {
                return data.RequestBodyLengthBytes;
            });
            var avgSize = RequestProfilerMiddleware.ProfiledRequests.Average((data) => {
                return data.RequestBodyLengthBytes;
            });

            // This could be improved by intelligently detecting a "Drop Point" in the target HTML
            var toInject = Encoding.ASCII.GetBytes($"Min size: {shortest}, Average size: {avgSize}, Max size: {longest}.");

            return toInject;
        }
    }
}
