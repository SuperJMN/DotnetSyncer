namespace DotnetSyncer.Console.Core;

public interface IPlugin
{
    string Name { get; }
    string FriendlyName { get; }
    Task<Result<ISyncFileSystem>> Create(string args);
}