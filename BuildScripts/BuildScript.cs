using System;
using System.IO;
using Flubu;
using Flubu.Builds;
using Flubu.Builds.Tasks.AnalysisTasks;
using Flubu.Builds.VSSolutionBrowsing;
using Flubu.Targeting;

//css_inc CompileSolutionTask;
//css_inc NUnitWithDotCoverTask;
//css_inc NUnitTask;
//css_inc PublishNuGetPackageTask;
//css_inc StringEx;
//css_ref System.Xml;
//css_ref Flubu.dll;
//css_ref Flubu.Contrib.dll;

namespace BuildScripts
{
    public class BuildScript
    {
        public static int Main(string[] args)
        {
            DefaultBuildScriptRunner runner = new DefaultBuildScriptRunner(ConstructTargets, ConfigureBuildProperties);
            return runner.Run(args);
        }

        private static void ConstructTargets (TargetTree targetTree)
        {
            targetTree.AddTarget ("rebuild")
                .SetAsDefault ()
                .SetDescription ("Builds the library and runs tests on it")
                .DependsOn ("compile-4.0", "compile-3.5", "dupfinder", "tests-4.0", "tests-3.5");

            targetTree.AddTarget("release")
                .SetDescription ("Builds the library, runs tests on it and publishes it on the NuGet server")
                .DependsOn ("rebuild", "nuget");

            targetTree.GetTarget ("fetch.build.version")
                .Do (TargetFetchBuildVersion);

            targetTree.AddTarget("compile-4.0")
                .SetAsHidden()
                .DependsOn("load.solution")
                .Do(TargetCompile40);
            
            targetTree.AddTarget("compile-3.5")
                .SetAsHidden()
                .DependsOn("load.solution")
                .Do(TargetCompile35);

            targetTree.AddTarget ("dupfinder")
                .SetDescription ("Runs R# dupfinder to find code duplicates")
                .Do (TargetDupFinder);

            targetTree.AddTarget("tests-4.0")
                .SetDescription("Runs tests on the .NET 4.0-targeted assemblies (+ measuring code coverage)")
                .Do (r =>
                    {
                        TargetRun40TestsWithCoverage(r, "LibroLib.Tests");
                    }).DependsOn ("load.solution");

            targetTree.AddTarget("tests-3.5")
                .SetDescription("Runs tests on the .NET 3.5-targeted assemblies")
                .Do (r =>
                    {
                        TargetRun35Tests(r, "LibroLib.Tests");
                    }).DependsOn ("load.solution");

            targetTree.AddTarget ("nuget")
                .SetDescription ("Produces NuGet packages for the library and publishes them to the NuGet server")
                .Do (c =>
                {
                    TargetNuGet(c, "LibroLib.Common");
                    TargetNuGet(c, "LibroLib.WebUtils");
                }).DependsOn ("fetch.build.version");
        }

        private static void ConfigureBuildProperties (TaskSession session)
        {
            session.Properties.Set (BuildProps.CompanyName, "Igor Brejc");
            session.Properties.Set (BuildProps.CompanyCopyright, "Copyright (C) 2010-2015 Igor Brejc");
            session.Properties.Set (BuildProps.ProductId, "LibroLib");
            session.Properties.Set (BuildProps.ProductName, "LibroLib");
            session.Properties.Set (BuildProps.SolutionFileName, "LibroLib.sln");
            session.Properties.Set (BuildProps.TargetDotNetVersion, FlubuEnvironment.Net40VersionNumber);
            session.Properties.Set (BuildProps.VersionControlSystem, VersionControlSystem.Mercurial);
            session.Properties.Set (BuildProps.BuildConfiguration, "Release-4.0");
        }

        private static void TargetFetchBuildVersion (ITaskContext context)
        {
            Version version = BuildTargets.FetchBuildVersionFromFile (context);
            version = new Version (version.Major, version.Minor, BuildTargets.FetchBuildNumberFromFile (context));
            context.Properties.Set (BuildProps.BuildVersion, version);
            context.WriteInfo ("The build version will be {0}", version);
        }

        private static void TargetCompile35(ITaskContext context)
        {
            CompileSolutionTask task = new CompileSolutionTask (
                context.Properties.Get<VSSolution>(BuildProps.Solution).SolutionFileName.ToString (), 
                "Release-3.5", 
                context.Properties.Get<string>(BuildProps.TargetDotNetVersion));

            task.MaxCpuCount = context.Properties.Get("CompileMaxCpuCount", 3);
            task.Target = "Rebuild";
            
            task.Execute(context);
        }

        private static void TargetCompile40(ITaskContext context)
        {
            CompileSolutionTask task = new CompileSolutionTask (
                context.Properties.Get<VSSolution>(BuildProps.Solution).SolutionFileName.ToString (), 
                "Release-4.0", 
                context.Properties.Get<string>(BuildProps.TargetDotNetVersion));

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
                Path.Combine(projectName, "bin", context.Properties[BuildProps.BuildConfiguration], projectName) + ".dll",
                @"packages\NUnit.Runners.2.6.4\tools\nunit-console.exe");
            task.DotCoverFilters = "-:module=*.Tests;-:class=*Contract;-:class=*Contract`*";
            task.NUnitCmdLineOptions = "/framework:4.0 /labels /nodots";
            task.FailBuildOnViolations = false;
            task.Execute (context);
        }

        private static void TargetRun35Tests (ITaskContext context, string projectName)
        {
            string testAssemblyDir = Path.Combine(projectName, "bin", "Release-3.5");
            string testAssemblyFileName = projectName + ".dll";

            NUnitTask task = new NUnitTask(testAssemblyFileName, @"packages\NUnit.Runners.2.6.4\tools\nunit-console.exe", testAssemblyDir);
            task.TargetFramework = "3.5";
            task.Execute (context);
        }

        private static void TargetNuGet (ITaskContext context, string projectName)
        {
            string nuspecFileName = Path.Combine(projectName, projectName) + ".nuspec";

            PublishNuGetPackageTask publishTask = new PublishNuGetPackageTask (
                projectName, nuspecFileName);
            publishTask.BasePath = Path.GetFullPath(Path.Combine(
                projectName, "bin", context.Properties[BuildProps.BuildConfiguration]));
            publishTask.ForApiKeyUseEnvironmentVariable ();
            publishTask.Execute (context);
        }
    }
}