using System;
using System.Linq;
using CSharpFunctionalExtensions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using Zafiro.Nuke;

class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitVersion] readonly GitVersion GitVersion;
    [Parameter] [Secret] readonly string NuGetApiKey;
    [GitRepository] readonly GitRepository Repository;
    [Solution] readonly Solution Solution;

    Target Clean => d => d
        .Executes(() =>
        {
            var absolutePaths = RootDirectory.GlobDirectories("**/bin", "**/obj").Where(a => !((string)a).Contains("build")).ToList();
            Log.Information("Deleting {Dirs}", absolutePaths);
            absolutePaths.DeleteDirectories();
        });

    Target PublishNuget => d => d
        .Requires(() => NuGetApiKey)
        .DependsOn(Clean)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            var actions = new Actions(Solution, Repository, RootDirectory, GitVersion, Configuration);
            actions.PushNuGetPackages(NuGetApiKey)
                .TapError(error => throw new ApplicationException(error));
        });

    public static int Main() => Execute<Build>(x => x.PublishNuget);
}