using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace middleware
{
    /// Metadata about a request
    public class RequestProfiledModel
    {
        public RequestProfiledModel(TimeSpan duration, long length)
        {
            this.RequestDuration = duration;
            this.RequestBodyLengthBytes = length;
        }
        
        /// How long the request took to execute
        public TimeSpan RequestDuration {get; set;}

        /// The size of the request (pre-transformation) in bytes
        public long RequestBodyLengthBytes {get; set;}
    }
}
