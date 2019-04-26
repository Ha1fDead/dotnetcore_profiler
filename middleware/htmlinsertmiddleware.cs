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

        public async Task InvokeAsync(HttpContext context)
        {
            var capturedBody = context.Response.Body;
            using (var updatedBody = new MemoryStream())
            {
                context.Response.Body = updatedBody;
                await _requestDelegate.Invoke(context);
                context.Response.Body = capturedBody;

                var shortest = RequestDataCollectorMiddleware.RequestTimes.Min();
                var longest = RequestDataCollectorMiddleware.RequestTimes.Max();
                var avgTicks = RequestDataCollectorMiddleware.RequestTimes.Average((data) => {
                    return data.Ticks;
                });
                var average = TimeSpan.FromTicks(Convert.ToInt64(avgTicks));

                var length = Encoding.ASCII.GetBytes($"Min: {shortest}, Average: {average}, Max: {longest}");
                await updatedBody.WriteAsync(length, 0, length.Length);
                updatedBody.Seek(0, SeekOrigin.Begin);

                var newContent = new StreamReader(updatedBody).ReadToEnd();
                context.Response.ContentLength = context.Response.ContentLength + length.Length;
                await context.Response.WriteAsync(newContent);
            }

        }
    }
}
