using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using Xunit;

namespace middleware.test
{
    public class ProfilerLogicTest
    {        
        [Fact]
        public void GetInjected_NothingToInject_Empty()
        {
            // Arrange
            ProfilerLogic.ProfiledRequests = new ConcurrentBag<RequestProfiledModel>();

            // Act
            var res = ProfilerLogic.GetInjected();

            // Assert
            Assert.NotNull(res);
            Assert.Empty(res);
        }

        [Fact]
        public void GetInjected_ProperlyEncodesBinaryData()
        {
            // Arrange
            ProfilerLogic.ProfiledRequests = new ConcurrentBag<RequestProfiledModel>();
            ProfilerLogic.ProfiledRequests.Add(new RequestProfiledModel(TimeSpan.FromDays(1), 1000)); // longest
            ProfilerLogic.ProfiledRequests.Add(new RequestProfiledModel(TimeSpan.FromDays(1), 100)); 
            ProfilerLogic.ProfiledRequests.Add(new RequestProfiledModel(TimeSpan.FromDays(1), 10)); // min

            // Act
            var res = ProfilerLogic.GetInjected();

            // Assert
            Assert.NotNull(res);
            Assert.NotEmpty(res);
            var message = Encoding.ASCII.GetString(res);
            Assert.NotEmpty(message);
            Assert.Equal("Min size: 10, Average size: 370, Max size: 1000.", message);
        }
    }
}
