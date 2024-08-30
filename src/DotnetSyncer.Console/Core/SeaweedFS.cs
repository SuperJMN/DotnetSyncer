using System.Diagnostics;
using Microsoft.Extensions.Http.Logging;
using Serilog;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Misc;
using IDirectory = Zafiro.FileSystem.Readonly.IDirectory;
using IFile = Zafiro.FileSystem.Readonly.IFile;

namespace DotnetSyncer.Console.Core;

public class SeaweedFS : ISyncFileSystem
{
    public Maybe<ILogger> Logger { get; }
    private readonly ISeaweedFS seaweedFSClient;

    private SeaweedFS(ISeaweedFS seaweedFSClient, Maybe<ILogger> logger)
    {
        Logger = logger;
        this.seaweedFSClient = seaweedFSClient;
    }

    public string Name => "seaweedfs";
    public string DisplayName => "SeaweedFS";

    public Task<Result<IDirectory>> GetFiles(ZafiroPath path)
    {
        return Zafiro.FileSystem.SeaweedFS.Directory.From(path, seaweedFSClient)
            .Bind(x => x.ToDirectory());
    }

    public Task<Result> Copy(IFile left, ZafiroPath destinationFileName)
    {
        return Result.Try(async () =>
        {
            await using var stream = left.ToStream();
            await seaweedFSClient.Upload(destinationFileName, stream);
        });
    }

    public static async Task<Result<SeaweedFS>> Create(string baseUri, Maybe<ILogger> logger)
    {
        return Result.Try(() =>
        {
            var httpClient = GetHttpClient(baseUri, logger);
            var seaweedFSClient = new SeaweedFSClient(httpClient);
            return new SeaweedFS(seaweedFSClient, logger);
        });
    }

    private static HttpClient GetHttpClient(string baseAddress, Maybe<ILogger> logger)
    {
        var handler = GetHandler(logger);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress), Timeout = TimeSpan.FromHours(5),
        };
    }

    private static HttpMessageHandler GetHandler(Maybe<ILogger> logger)
    {
        if (logger.HasValue && Debugger.IsAttached)
        {
            return new LoggingHttpMessageHandler(new LoggerAdapter(logger.Value))
            {
                InnerHandler = new HttpClientHandler()
            };
        }

        return new HttpClientHandler();
    }

    public override string ToString() => DisplayName;
}