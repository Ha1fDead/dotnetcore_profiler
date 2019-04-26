using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace middleware
{
    public static class ProfilerLogic
    {
        // What happens if the server uptime is measured in decades, with millions of requests per second?
        // Global State in libraries is hard -- not sure what the best practices are
        // This will reset on app restart or app refresh
        // I think I'd want a permanent storage for this so data wouldn't be lost
        public static ConcurrentBag<RequestData> ProfiledRequests = new ConcurrentBag<RequestData>();

        public static Byte[] GetInjected() {
            if (!ProfilerLogic.ProfiledRequests.Any())
            {
                return new Byte[]{};
            }

            var shortest = ProfilerLogic.ProfiledRequests.Min((data) => {
                return data.RequestBodyLengthBytes;
            });
            var longest = ProfilerLogic.ProfiledRequests.Max((data) => {
                return data.RequestBodyLengthBytes;
            });
            var avgSize = ProfilerLogic.ProfiledRequests.Average((data) => {
                return data.RequestBodyLengthBytes;
            });

            // This could be improved by intelligently detecting a "Drop Point" in the target HTML
            var toInject = Encoding.ASCII.GetBytes($"Min size: {shortest}, Average size: {avgSize}, Max size: {longest}.");
            return toInject;
        }
    }
}
