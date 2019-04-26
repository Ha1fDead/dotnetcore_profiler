# .NET Instrumentation Engineer Projec

https://ci.appveyor.com/api/projects/status/pcgoxda2pmfwqc3j/branch/master?svg=true

Helpful:


https://github.com/aspnet/AspNetCore/issues/664
https://stackoverflow.com/questions/30652138/how-to-retrieve-the-current-response-body-length
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.0 
    (very very basic high level overview)
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/request-response?view=aspnetcore-3.0 
    (how to manipulate request/response in middleware)
https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/ 
    (super in-depth on pipeline IO and relevant for performance)
http://anthonygiretti.com/2018/09/04/asp-net-core-2-1-middlewares-part1-building-a-custom-middleware/
    (mildly beginner-friendly guide)

https://github.com/aspnet/BrowserLink/blob/master/src/Microsoft.VisualStudio.Web.BrowserLink/BrowserLinkMiddleWare.cs
    Fantastic actual source code for doing something very similar (writing to arbitrary response bodies)

https://jeremylindsayni.wordpress.com/2019/02/18/adding-middleware-to-your-net-core-mvc-pipeline-that-formats-and-indents-html-output/
    Helpful article that taught me how to intercept the response stream