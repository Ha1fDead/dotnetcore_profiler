using System;
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
        public static List<TimeSpan> RequestTimes = new List<TimeSpan>();
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
