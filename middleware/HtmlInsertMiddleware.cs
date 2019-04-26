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
            var capturedBody = context.Response.Body;
            
            // What happens if someone has some content already written to the response?
            // Pretty sure this shouldn't be possible given middleware architecture -- not supposed to do anything with the request until the way back out of the middleware
            try
            {
                using (var swapBody = new MemoryStream())
                {
                    context.Response.Body = swapBody;
                    await _requestDelegate.Invoke(context);

                    if (RequestDataCollectorMiddleware.RequestTimes.Any())
                    {
                        var shortest = RequestDataCollectorMiddleware.RequestTimes.Min((data) => {
                            return data.RequestBodyLengthBytes;
                        });
                        var longest = RequestDataCollectorMiddleware.RequestTimes.Max((data) => {
                            return data.RequestBodyLengthBytes;
                        });
                        var avgSize = RequestDataCollectorMiddleware.RequestTimes.Average((data) => {
                            return data.RequestBodyLengthBytes;
                        });

                        // This could be improved by intelligently detecting a "Drop Point" in the target HTML
                        var length = Encoding.ASCII.GetBytes($"Min size: {shortest}, Average size: {avgSize}, Max size: {longest}.");
                        await swapBody.WriteAsync(length, 0, length.Length);
                        context.Response.ContentLength = swapBody.Length + length.Length;
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
    }
}
