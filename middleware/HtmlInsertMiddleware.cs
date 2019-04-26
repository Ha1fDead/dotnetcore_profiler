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
            capturedBody.Dispose();
            
            using (var updatedBody = new MemoryStream())
            {
                context.Response.Body = updatedBody;
                await _requestDelegate.Invoke(context);
                context.Response.Body = capturedBody;

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
                    await updatedBody.WriteAsync(length, 0, length.Length);
                    context.Response.ContentLength = updatedBody.Length + length.Length;
                }

                updatedBody.Seek(0, SeekOrigin.Begin);
                var newContent = new StreamReader(updatedBody).ReadToEnd();
                await context.Response.WriteAsync(newContent);
            }

        }
    }
}
