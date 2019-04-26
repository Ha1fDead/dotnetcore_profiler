using System;
using Xunit;

namespace middleware.test
{
    public class HtmlInsertMiddleware
    {
        [Fact]
        public void works()
        {
            Assert.True(true);
        }
        
        [Fact(Skip = "Scaffold")]
        public void InvokeAsync_Html_ProperlyInsertsHtml()
        {
            Assert.True(false);
        }

        [Fact(Skip = "Scaffold")]
        public void InvokeAsync_NotValidType_DoesNotInsertHtml()
        {

        }
    }
}
