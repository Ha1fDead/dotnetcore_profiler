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
    public class RequestDataCollectorMiddleware
    {
        // What happens if the server uptime is measured in decades, with millions of requests per second?
        public static ConcurrentBag<TimeSpan> RequestTimes = new ConcurrentBag<TimeSpan>();
        private readonly RequestDelegate _requestDelegate;
        public RequestDataCollectorMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            await _requestDelegate.Invoke(context);
            sw.Stop();

            // definitely not thread safe
            RequestDataCollectorMiddleware.RequestTimes.Add(sw.Elapsed);
        }
    }
}
