// See https://aka.ms/new-console-template for more information

using AvaloniaSyncer.Console;
using AvaloniaSyncer.Console.Core;
using CommandLine;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;

var plugins = new IPlugin[] { new DotnetFsPlugin(), new SeaweedFSPlugin() };

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

await Parser.Default
    .ParseArguments<Options>(args)
    .WithParsedAsync(options =>
    {
        var factory = new FileSourceFactory(plugins);
        var leftSource = factory.GetFileSource(options.Left);
        var rightSource = factory.GetFileSource(options.Right);

        return leftSource
            .CombineAndBind(rightSource, (left, right) => new Syncer(Maybe.From(Log.Logger)).Sync(left, right))
            .Log();
    });