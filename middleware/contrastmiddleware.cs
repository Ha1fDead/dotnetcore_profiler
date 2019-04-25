using System;
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
        
        Goals:

        Measure the total time spent processing the request.
        Measure the size of the response body in bytes. Calculate the minimum, average, and maximum responses seen so far.
        Add new content to HTML pages to display information gathered by your project.
        Ensure that your IHttpModule or Middleware is thread-safe and can correctly handle multiple concurrent requests.
        Handle multiple encodings and different types of pages.
        Use server resources (i.e. memory and processor) efficiently.
        Include a small web application that demonstrates the behavior of your IHttpModule (or Middleware.) This web application is not the focus of the project and you should feel free to use the web application template projects provided by Visual Studio.

         */
        public async Task Invoke(HttpContext context)
        {
            // Request Delegates run in the order they are registered
            // So we need to ensure ours runs first
            DateTime start = DateTime.UtcNow;
            await _requestDelegate.Invoke(context);
            DateTime end = DateTime.UtcNow;

            Console.WriteLine((end - start).ToString());

        }
    }
}
