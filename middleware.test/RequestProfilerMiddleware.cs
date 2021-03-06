using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Xunit;

namespace middleware.test
{
    public class RequestDataCollectionMiddleware
    {
        [Fact()]
        public async void Invoke_RecordsRuntime()
        {
            // Arrange
            const string mockResponse = "This_is_a_test";
            var requestProfilerMiddleware = new RequestProfilerMiddleware(async (innerHttpContext) => {
                await innerHttpContext.Response.WriteAsync(mockResponse);
                innerHttpContext.Request.ContentLength = Encoding.ASCII.GetByteCount(mockResponse);

                // reset the static global so test runs correctly
                ProfilerLogic.ProfiledRequests = new System.Collections.Concurrent.ConcurrentBag<RequestProfiledModel>();
            });

            var contextStub = new DefaultHttpContext();
            var bodyStub = new MemoryStream();
            contextStub.Features.Get<IHttpResponseFeature>().Body = bodyStub;

            // Act
            await requestProfilerMiddleware.InvokeAsync(contextStub);

            // Assert
            Assert.NotEmpty(ProfilerLogic.ProfiledRequests);
            Assert.Equal(1, ProfilerLogic.ProfiledRequests.Count);
            var profiledResponse = ProfilerLogic.ProfiledRequests.First();
            Assert.Equal(Encoding.ASCII.GetByteCount(mockResponse), profiledResponse.RequestBodyLengthBytes);
            Assert.NotEqual(0, profiledResponse.RequestDuration.Ticks);
        }
    }
}
