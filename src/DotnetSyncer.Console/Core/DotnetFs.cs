using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.Local.Mutable;
using IDirectory = Zafiro.FileSystem.Readonly.IDirectory;
using IFile = Zafiro.FileSystem.Readonly.IFile;

namespace DotnetSyncer.Console.Core;

public class DotnetFs : ISyncFileSystem
{
    private readonly DotNetMutableFileSystem fs;

    private DotnetFs()
    {
        fs = new DotNetMutableFileSystem(new FileSystem());
    }

    public string Name => "local";
    public string DisplayName => "Local Filesystem";

    public Task<Result<IDirectory>> GetFiles(ZafiroPath path)
    {
        return fs.GetDirectory(path).Bind(x => x.ToDirectory());
    }

    public Task<Result> Copy(IFile left, ZafiroPath destination)
    {
        return fs.GetFile(destination).Bind(rooted => rooted.SetContents(left));
    }

    public static async Task<Result<DotnetFs>> Create()
    {
        return new DotnetFs();
    }

    public override string ToString() => DisplayName;
}