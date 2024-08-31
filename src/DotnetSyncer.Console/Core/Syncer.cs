using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Comparer;

namespace DotnetSyncer.Console.Core;

public class Syncer
{
    private readonly Maybe<ILogger> logger;

    public Syncer(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }

    public async Task<Result> Sync(FileSource left, FileSource right)
    {
        logger.Execute(l => l.Information("Syncing {Left} and {Right}", left, right));

        var leftResult = await left.GetFiles().Map(x => x.RootedFiles());
        var rightResult = await right.GetFiles().Map(x => x.RootedFiles());

        return await leftResult
            .CombineAndMap(rightResult, (l, r) => l.Diff(r))
            .Bind(s => s.Select(diff => ProcessDiff(diff, left, right)).Combine());
    }

    private Task<Result> ProcessDiff(FileDiff diff, FileSource left, FileSource right)
    {
        switch (diff)
        {
            case BothDiff bothDiff:
                return OnBothDiff(bothDiff, left, right);
            case LeftOnlyDiff leftOnlyDiff:
                return LeftOnly(leftOnlyDiff, left, right);
            case RightOnlyDiff rightOnlyDiff:
                return RightOnly(rightOnlyDiff, left, right);
            default:
                throw new ArgumentOutOfRangeException(nameof(diff));
        }
    }

    private async Task<Result> OnBothDiff(BothDiff bothDiff, FileSource leftPath, FileSource rightPath)
    {
        // TODO: Review what happens when file is in both locations
        return Result.Success();
    }

    private async Task<Result> LeftOnly(LeftOnlyDiff leftOnlyDiff, FileSource left, FileSource right)
    {
        var result = await right.Plugin.Copy(leftOnlyDiff.Left, right.Path.Combine(leftOnlyDiff.Left.FullPath()));
        logger.Execute(l => l.Information("Left only: {Left} => {Right} : {Name}", left, right, leftOnlyDiff.Left));
        result.TapError(err => logger.Execute(l => l.Error(err)));
        return result;
    }

    private async Task<Result> RightOnly(RightOnlyDiff rightOnlyDiff, FileSource left, FileSource right)
    {
        logger.Execute(l => l.Information("Right only: {Left} => {Right} : {Name} ", left, right, rightOnlyDiff.Right));
        return Result.Success();
    }
}