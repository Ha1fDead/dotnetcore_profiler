using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace middleware
{
    public class RequestProfilerMiddleware
    {
        // What happens if the server uptime is measured in decades, with millions of requests per second?
        // Global State in libraries is hard -- not sure what the best practices are
        // This will reset on app restart or app refresh
        // I think I'd want a permanent storage for this so data wouldn't be lost
        public static ConcurrentBag<RequestData> ProfiledRequests = new ConcurrentBag<RequestData>();
        private readonly RequestDelegate _requestDelegate;
        public RequestProfilerMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            await _requestDelegate.Invoke(context);
            sw.Stop();

            var model = new RequestData(sw.Elapsed, context.Response.Body.Length);
            RequestProfilerMiddleware.ProfiledRequests.Add(model);
        }
    }
}
