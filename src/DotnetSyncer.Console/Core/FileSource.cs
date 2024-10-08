﻿using IDirectory = Zafiro.FileSystem.Readonly.IDirectory;

namespace DotnetSyncer.Console.Core;

public record FileSource
{
    public FileSource(ISyncFileSystem plugin, ZafiroPath path)
    {
        Plugin = plugin;
        Path = path;
    }

    public ISyncFileSystem Plugin { get; }
    public ZafiroPath Path { get; }

    public Task<Result<IDirectory>> GetFiles()
    {
        return Plugin.GetFiles(Path);
    }

    public override string ToString()
    {
        return $"[{Plugin}]:{Path}";
    }
}