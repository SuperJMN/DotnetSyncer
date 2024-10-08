﻿using Serilog;

namespace DotnetSyncer.Console.Core;

public class FileSourceFactory
{
    public FileSourceFactory(IEnumerable<IPlugin> plugins, Maybe<ILogger> logger)
    {
        Plugins = plugins;
        Logger = logger;
    }

    public IEnumerable<IPlugin> Plugins { get; }
    public Maybe<ILogger> Logger { get; }

    public Task<Result<FileSource>> GetFileSource(string str)
    {
        var split = str.Split("|");

        return Result.Success(split)
            .Ensure(x => x.Length == 3, $"Descriptor \"{str}\" should have exactly 3 parts separated by |")
            .Map(parts =>
            {
                var pluginName = parts[0];
                var args = parts[1];
                return Plugins.TryFirst(x => x.Name.Equals(pluginName))
                    .ToResult("Plugin not found")
                    .Map(plugin => plugin.Create(args, Logger))
                    .Map(x => new FileSource(x, parts[2]));
            });
    }
}