using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace middleware
{
    public class ContrastMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public ContrastMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        /**
        https://devblogs.microsoft.com/dotnet/custom-asp-net-core-middleware-example/
        Goals:

        Measure the total time spent processing the request.
        Measure the size of the response body in bytes. Calculate the minimum, average, and maximum responses seen so far.
        Add new content to HTML pages to display information gathered by your project.
        Ensure that your IHttpModule or Middleware is thread-safe and can correctly handle multiple concurrent requests.
        Handle multiple encodings and different types of pages.
        Use server resources (i.e. memory and processor) efficiently.
        Include a small web application that demonstrates the behavior of your IHttpModule (or Middleware.) This web application is not the focus of the project and you should feel free to use the web application template projects provided by Visual Studio.

         */
        public async Task InvokeAsync(HttpContext context)
        {
            // Request Delegates run in the order they are registered
            // So we need to ensure ours runs first
            var capturedBody = context.Response.Body;
            var sw = new Stopwatch();
            sw.Start();

            using (var updatedBody = new MemoryStream())
            {
                context.Response.Body = updatedBody;
                await _requestDelegate.Invoke(context);
                sw.Stop();

                context.Response.Body = capturedBody;

                var length = Encoding.ASCII.GetBytes($"Run Duration: {sw.ElapsedMilliseconds}");
                await updatedBody.WriteAsync(length, 0, length.Length);
                updatedBody.Seek(0, SeekOrigin.Begin);
                var newContent = new StreamReader(updatedBody).ReadToEnd();
                context.Response.ContentLength = context.Response.ContentLength + length.Length;
                await context.Response.WriteAsync(newContent);
            }

        }
    }
}
