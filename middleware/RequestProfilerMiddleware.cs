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
            ProfilerLogic.ProfiledRequests.Add(model);
        }
    }
}
