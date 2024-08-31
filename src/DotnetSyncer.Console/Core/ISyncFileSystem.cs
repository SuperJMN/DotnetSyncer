using IDirectory = Zafiro.FileSystem.Readonly.IDirectory;
using IFile = Zafiro.FileSystem.Readonly.IFile;

namespace DotnetSyncer.Console.Core;

public interface ISyncFileSystem
{
    string Name { get; }
    string DisplayName { get; }
    Task<Result<IDirectory>> GetFiles(ZafiroPath path);
    Task<Result> Copy(IFile left, ZafiroPath destination);
}