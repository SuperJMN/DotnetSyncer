using Serilog;

namespace DotnetSyncer.Console.Core;

public class DotnetFsPlugin : IPlugin
{
    public string Name => "local";
    public string FriendlyName => "Local Filesystem";

    public Task<Result<ISyncFileSystem>> Create(string args, Maybe<ILogger> logger)
    {
        return DotnetFs.Create().Map(x => (ISyncFileSystem)x);
    }
}