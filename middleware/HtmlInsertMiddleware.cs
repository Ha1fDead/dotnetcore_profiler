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

                    var injected = ProfilerLogic.GetInjected();
                    await swapBody.WriteAsync(injected, 0, injected.Length);
                    context.Response.ContentLength = swapBody.Length + injected.Length;

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
