# .NET Instrumentation Engineer Project

[![Build status](https://ci.appveyor.com/api/projects/status/pcgoxda2pmfwqc3j/branch/master?svg=true)](https://ci.appveyor.com/project/NathanLafferty/dotnetcore-profiler/branch/master)

This is a coding exercise ran by [Contrast-Security](https://contrast-security-oss.github.io/join-the-team/challenges.html)

The goal of the project was to build a very simple ASP.NET Core profiler using middleware that could determine body lengths and manipulate generated html.

## Building

Requirements:

1. `dotnetcore` SDK installed ([download](https://dotnet.microsoft.com/download))
2. clone the project (`git clone https://github.com/Ha1fDead/dotnetcore_profiler.git`)
3. run `dotnet run` from the `rest_file_api` directory (NOT the root solution)

(Note: Your browser will not automatically load from this setup)

(You can run `dotnet --info` to verify dotnet core is installed correctly)

For Visual Studio Code:

1. `dotnetcore` SDK installed ([download](https://dotnet.microsoft.com/download))
2. Install the [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)
3. clone the project (`git clone https://github.com/Ha1fDead/dotnetcore_profiler.git`)
4. Open the solution in VSCode
5. Hit F5, your default browser should open

The example profiled app should be running on `http://localhost:5000` and `https://localhost:5001`.

## Testing

Run `dotnet test` from the solution directory or the tests project.

Or install [NET Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer)

NOTE: Currently one of the tests fails unless ran in isolation. This is due to a choice in state pattern, and is discussed in `problems` section.

## Using

Install project and build (see previous step).

Copy and reference the `middleware.dll` file into your project.

Add desired middleware:

`app.UseHtmlInsertMiddleware();` will inject profiled data into all of your `text/html` pages
`app.UseRequestProfilerMiddleware();` will actually profile your requests for how long they take and their response body sizes. Does *not* alter your html pages.

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
7. Autolink appveyer build to release `.dll` file generation
8. Provide very simple way to install package (nuget, etc.)

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

1. How does HTTP2 affect the system? (I don't think it will)
2. How do file transfers affect the system? (particularly large streamed files)
3. How does streaming content affect the system?
    - Large-data transfers
    - Websockets
4. Are there any security considerations with profiling response sizes with encryption keys?
5. How does this app function if ran in ASP.NET 4.7.2 or older versions?