using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Flubu;
using Flubu.Builds;
using Flubu.Builds.Tasks.AnalysisTasks;
using Flubu.Builds.Tasks.NuGetTasks;
using Flubu.Builds.Tasks.SolutionTasks;
using Flubu.Builds.Tasks.TestingTasks;
using Flubu.Builds.VSSolutionBrowsing;
using Flubu.Targeting;

//css_inc StringEx;

namespace BuildScripts
{
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties (TaskSession session)
        {
            session.Properties.Set (BuildProps.CompanyName, "Igor Brejc");
            session.Properties.Set (BuildProps.CompanyCopyright, "Copyright (C) 2010-2017 Igor Brejc");
            session.Properties.Set (BuildProps.ProductId, "LibroLib");
            session.Properties.Set (BuildProps.ProductName, "LibroLib");
            session.Properties.Set (BuildProps.SolutionFileName, "LibroLib.sln");
            session.Properties.Set (BuildProps.MSBuildToolsVersion, "14.0");
            session.Properties.Set (BuildProps.VersionControlSystem, VersionControlSystem.Mercurial);
            session.Properties.Set (BuildProps.BuildConfiguration, "Release-4.0");
        }

        protected override void ConfigureTargets(TargetTree targetTree, ICollection<string> args)
        {
            targetTree.AddTarget ("rebuild")
                .SetAsDefault ()
                .SetDescription ("Builds the library and runs tests on it")
                .DependsOn ("compile-4.0", "dupfinder", "tests-4.0");

            targetTree.AddTarget("release")
                .SetDescription ("Builds the library, runs tests on it and publishes it on the NuGet server")
                .DependsOn ("rebuild", "nuget");

            targetTree.GetTarget ("fetch.build.version")
                .Do (TargetFetchBuildVersion);

            targetTree.AddTarget("compile-4.0")
                .SetAsHidden()
                .DependsOn ("load.solution", "generate.commonassinfo")
                .Do(TargetCompile40);

            targetTree.AddTarget ("dupfinder")
                .SetDescription ("Runs R# dupfinder to find code duplicates")
                .Do (TargetDupFinder);

            targetTree.AddTarget("tests-4.0")
                .SetDescription("Runs tests on the .NET 4.0-targeted assemblies (+ measuring code coverage)")
                .Do (r =>
                    {
                        TargetRun40TestsWithCoverage(r, "LibroLib.Tests");
                    }).DependsOn ("load.solution");

            targetTree.AddTarget ("nuget")
                .SetDescription ("Produces NuGet packages for the library and publishes them to the NuGet server")
                .Do (c =>
                {
                    TargetNuGet(c, "LibroLib.Common");
                    TargetNuGet(c, "LibroLib.WebUtils");
                }).DependsOn ("fetch.build.version");
        }

        private static void TargetFetchBuildVersion (ITaskContext context)
        {
            Version version = BuildTargets.FetchBuildVersionFromFile (context);
            context.Properties.Set (BuildProps.BuildVersion, version);
            context.WriteInfo ("The build version will be {0}", version);
        }

        private static void TargetCompile40(ITaskContext context)
        {
            CompileSolutionTask task = new CompileSolutionTask (
                context.Properties.Get<VSSolution>(BuildProps.Solution).SolutionFileName.ToString (), 
                "Release-4.0");

            task.MaxCpuCount = context.Properties.Get("CompileMaxCpuCount", 3);
            task.Target = "Rebuild";
            
            task.Execute(context);
        }

        private static void TargetDupFinder (ITaskContext context)
        {
            RunDupFinderAnalysisTask task = new RunDupFinderAnalysisTask ();
            task.Execute (context);
        }

        private static void TargetRun40TestsWithCoverage (ITaskContext context, string projectName)
        {
            NUnitWithDotCoverTask task = new NUnitWithDotCoverTask(
                @"packages\NUnit.Runners.2.6.4\tools\nunit-console.exe",
                Path.Combine(projectName, "bin", context.Properties[BuildProps.BuildConfiguration], projectName) + ".dll");
            task.DotCoverFilters = "-:module=*.Tests;-:class=*Contract;-:class=*Contract`*;-:class=JetBrains.Annotations.*";
            task.NUnitCmdLineOptions = "/framework:4.0 /labels /nodots";
            task.FailBuildOnViolations = false;
            task.Execute(context);
        }

        private static void TargetNuGet (ITaskContext context, string projectName)
        {
            string nuspecFileName = Path.Combine(projectName, projectName) + ".nuspec";

            PublishNuGetPackageTask publishTask = new PublishNuGetPackageTask (
                projectName, nuspecFileName);
            publishTask.BasePath = Path.GetFullPath(projectName);
            publishTask.ForApiKeyUseEnvironmentVariable ();
            publishTask.NuGetServerUrl ="https://www.nuget.org/api/v2/package";
            publishTask.Execute (context);
        }
    }
}