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

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("invoked");

            await _requestDelegate.Invoke(context);
        }
    }
}
