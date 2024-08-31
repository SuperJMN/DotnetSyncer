// See https://aka.ms/new-console-template for more information

using CommandLine;
using DotnetSyncer.Console;
using DotnetSyncer.Console.Core;
using Serilog;
using Serilog.Events;
using Zafiro.CSharpFunctionalExtensions;

var plugins = new IPlugin[] { new DotnetFsPlugin(), new SeaweedFSPlugin() };

ILogger logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

await Parser.Default
    .ParseArguments<Options>(args)
    .WithParsedAsync(options =>
    {
        var factory = new FileSourceFactory(plugins, Maybe<ILogger>.None);
        var leftSource = factory.GetFileSource(options.Left);
        var rightSource = factory.GetFileSource(options.Right);

        return leftSource
            .CombineAndBind(rightSource, (left, right) => new Syncer(logger.AsMaybe()).Sync(left, right))
            .Log(logger: logger);
    });