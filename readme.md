# .NET Instrumentation Engineer Project

[![Build status](https://ci.appveyor.com/api/projects/status/pcgoxda2pmfwqc3j/branch/master?svg=true)](https://ci.appveyor.com/project/NathanLafferty/dotnetcore-profiler/branch/master)

## Future Improvements

1. Allow whitelist of developer-configurable content-type inserts
    - Developer can specify a whitelist such as `text/html`, `text/css` to output profiled data to
2. Create "Insertion Point" for developers to choose where data should be inserted into html
    - Currently its "dumbly" injected at the end of the response stream -- would improve ergonomics significantly to instead define a `profiler-point` for developers to use
3. Profile individual middlewares and their performance concerns. Probably impossible without using the .NET profiler APIs directly
4. Add support for gzip or brotli compression (see below -- encryption / compression)
5. Add user-configurable blacklist or whitelist for responses to profile
    - URL or session specific
    - Could add another form of developer-configurable middleware to provide further granular access based on session / authorization
6. Provide a mechanism to export application state to a file or database

## Problems

I only encountered one major problem with this, which is where to store state in library applications. I identified several approaches and tried to investigate best practices but, ultimately, there doesn't seem to be a "best approach" that doesn't have any cons.

1. Global Variable pattern (`static` variables)
    - Pro: very easy to code
    - Con: Global state, tests are difficult to write and run in isolation
2. Singleton Pattern
    - Pro: very easy to code
    - Con: More difficult to use than just global variable pattern
    - Con: Still doesn't solve the underlying test concerns
3. Registered ASP.NET Singleton pattern
    - Pro: Very easy to code
    - Pro: Very easy to test (injected dependency)
    - Con: Requires developers to register dependency manually or to provide a global registration. Either way leaky

I'd love to go over different approaches with other developers. I've never written any libraries that needed some form of state before, and its definitely an interesting architectural problem.

## UTF-Encoding

Full UTF support is handle by default. This is thanks to the use of pure binary data.

## Encryption & Compression

There are two major unknowns for the middleware: Encryption & Compression.

I decided to just assume neither of these are applicable, and instead opted for a simpler "Push the requirements to the developer to initialize the middleware properly".

- Encryption will modify content length and it is impossible to inject content into an encrypted file (also this is the responsibility of the underlying framework, *not* specific developer concerns)
- Utilizing compression is an implementation detail and injected uncompressed content into a compressed file would require unnecessary content

For both, I felt it was the responsibility of the underlying framework to handle these. Developers should register the middleware correctly such that its only dealing with unencrypted, uncompressed content to avoid unnecessary redundant computation.

## Unknowns

1. How does HTTP2 affect the system?
2. How do file transfers affect the system?
3. How does streaming content affect the system?
    - Large-data transfers
    - Websockets
4. Are there any security considerations with profiling response sizes with encryption keys?

## Helpful Resources


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