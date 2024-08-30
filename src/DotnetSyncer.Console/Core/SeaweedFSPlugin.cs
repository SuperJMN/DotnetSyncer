using Serilog;

namespace DotnetSyncer.Console.Core;

public class SeaweedFSPlugin : IPlugin
{
    public string Name => "seaweedfs";
    public string FriendlyName  => "SeaweedFS";
    public Task<Result<ISyncFileSystem>> Create(string args, Maybe<ILogger> logger) => SeaweedFS.Create(args, logger).Map(x => (ISyncFileSystem)x);
}