using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Rocket.Surgery.Nuke.ContinuousIntegration;
using Rocket.Surgery.Nuke.DotNetCore;
using Rocket.Surgery.Nuke.GithubActions;
using Serilog;
using System;
using System.Linq;

[EnsureGitHooks(GitHook.PreCommit)]
[EnsureReadmeIsUpdated("Readme.md")]
[DotNetVerbosityMapping]
[MSBuildVerbosityMapping]
[NuGetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
[LocalBuildConventions]
[AzurePipelinesSteps(AutoGenerate = true, InvokeTargets = new[] { nameof(Default) })]
[GitHubActionsSteps(
    "ci-ignore",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    On = new[] { RocketSurgeonGitHubActionsTrigger.Push, },
    OnPushTags = new[] { "v*", },
    OnPushBranches = new[] { "master", "main", "next", },
    OnPullRequestBranches = new[] { "master", "main", "next", }
)]
[GitHubActionsSteps(
    "ci",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    On = new[]
    {
        RocketSurgeonGitHubActionsTrigger.WorkflowCall,
        RocketSurgeonGitHubActionsTrigger.WorkflowDispatch,
    },
    OnPushTags = new[] { "v*", },
    OnPushBranches = new[] { "master", "main", "next", },
    OnPullRequestBranches = new[] { "master", "main", "next", },
    InvokedTargets = new[] { nameof(Default), },
    NonEntryTargets = new[]
    {
        nameof(ICIEnvironment.CIEnvironment),
        nameof(ITriggerCodeCoverageReports.TriggerCodeCoverageReports),
        nameof(ITriggerCodeCoverageReports.GenerateCodeCoverageReportCobertura),
        nameof(IGenerateCodeCoverageBadges.GenerateCodeCoverageBadges),
        nameof(IGenerateCodeCoverageReport.GenerateCodeCoverageReport),
        nameof(IGenerateCodeCoverageSummary.GenerateCodeCoverageSummary),
        nameof(Default),
    },
    ExcludedTargets = new[] { nameof(ICanClean.Clean), nameof(ICanRestoreWithDotNetCore.DotnetToolRestore), },
    Enhancements = new[] { nameof(CiMiddleware), }
)]
[GitHubActionsLint(
    "lint",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    OnPullRequestTargetBranches = new[] { "master", "main" },
    Enhancements = new[] { nameof(LintStagedMiddleware), }
)]
[PrintBuildVersion]
[PrintCIEnvironment]
[UploadLogs]
[TitleEvents]
[ContinuousIntegrationConventions]
public partial class Pipeline : NukeBuild,
    ICanRestoreWithDotNetCore,
    ICanBuildWithDotNetCore,
    ICanTestWithDotNetCore,
    ICanPackWithDotNetCore,
    IHaveDataCollector,
    ICanClean,
    ICanLintStagedFiles,
    ICanDotNetFormat,
    ICanPrettier,
    ICanUpdateReadme,
    IGenerateCodeCoverageReport,
    IGenerateCodeCoverageSummary,
    IGenerateCodeCoverageBadges,
    ICanRegenerateBuildConfiguration,
    IHaveConfiguration<Configuration>,
    IHavePublishArtifacts
{
    public static int Main() => Execute<Pipeline>(x => x.Default);

    Target Default => _ => _
        .DependsOn(Restore)
        .DependsOn(Build)
        .DependsOn(Test)
        .DependsOn(Pack)
        .Executes(() =>
        {
            Log.Information("nuke execution completed.");
        });

    [Solution]
    private Solution Solution { get; } = null!;

    public Target Build => _ => _.Inherit<ICanBuildWithDotNetCore>(x => x.CoreBuild);
    public Target Lint => _ => _.Inherit<ICanLint>(x => x.Lint);

    public Target Pack => _ => _
                              .Inherit<ICanPackWithDotNetCore>(x => x.CorePack)
                              .DependsOn(Clean);


    public Target Clean => _ => _.Inherit<ICanClean>(x => x.Clean);
    public Target Restore => _ => _.Inherit<ICanRestoreWithDotNetCore>(x => x.CoreRestore);
    Solution IHaveSolution.Solution => Solution;

    [GitVersion(NoFetch = false)]
    public GitVersion GitVersion { get; } = null!;

    public Target Test => _ => _.Inherit<ICanTestWithDotNetCore>(x => x.CoreTest);

    [Parameter("Configuration to build")]
    public Configuration Configuration { get; } = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    public static RocketSurgeonGitHubActionsConfiguration CiIgnoreMiddleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        ((RocketSurgeonsGithubActionsJob)configuration.Jobs[0]).Steps = new()
        {
            new RunStep("N/A")
            {
                Run = "echo \"No build required\"",
            },
        };

        return configuration.IncludeRepositoryConfigurationFiles();
    }

    public static RocketSurgeonGitHubActionsConfiguration CiMiddleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        configuration
           .ExcludeRepositoryConfigurationFiles()
           .AddNugetPublish()
           .Jobs.OfType<RocketSurgeonsGithubActionsJob>()
           .First(z => z.Name.Equals("Build", StringComparison.OrdinalIgnoreCase))
           .UseDotNetSdks("6.0", "8.0")
           .AddNuGetCache()
           // .ConfigureForGitVersion()
           .ConfigureStep<CheckoutStep>(step => step.FetchDepth = 0)
           .PublishLogs<Pipeline>();

        return configuration;
    }

    public static RocketSurgeonGitHubActionsConfiguration LintStagedMiddleware(RocketSurgeonGitHubActionsConfiguration configuration)
    {
        configuration
           .Jobs.OfType<RocketSurgeonsGithubActionsJob>()
           .First(z => z.Name.Equals("Build", StringComparison.OrdinalIgnoreCase))
           .UseDotNetSdks("6.0", "8.0");

        return configuration;
    }
}