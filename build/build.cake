#addin "Cake.FileHelpers&version=3.1.0"
#addin nuget:?package=Cake.ArgumentHelpers&version=0.3.0
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool "nuget:?package=ReportGenerator&version=4.0.14"

using System.Xml.XPath;

// Get Build Arguments
var runtime = ArgumentOrEnvironmentVariable("Runtime", "") ?? "win81-x64";
var revision = ArgumentOrEnvironmentVariable("Revision", "") ?? "1";

// Setup Parameters
var configuration = "Release";
var revisionSuffix = $".{revision}";
var fullVersion = FileReadText("version.txt") + revisionSuffix;
var deployEnvironment = "DEV";

// Log Parameters
Information("========================================");
Information("BUILD PARAMETERS");
Information("========================================");
Information($"Configuration: {configuration}");
Information($"Runtime: {runtime}");
Information($"Version: {fullVersion}");
Information($"Deploy To Environment: {deployEnvironment}");

DotNetCoreTestSettings TestSettings
{
    get
    {
        return new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            ArgumentCustomization = args => args.Append("-v normal"),
        };
    }
}

Task("Clean")
    .Does(() =>
    {
        CleanDirectories("../**/**/bin");
        Information("Cleaning 'bin' folders.");
        CleanDirectories("../**/**/obj");
        Information("Cleaning 'obj' folders.");
        CleanDirectories("../publish");
        Information("Cleaning 'publish' folder.");
        CleanDirectories("../Coverage");
        Information("Cleaning 'coverage' folder.");
    });

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(
            "../ShoppingCart.sln",
            new DotNetCoreRestoreSettings
            {
                Sources = new[]
                {
                    "https://api.nuget.org/v3/index.json",
                },
                Runtime = runtime,
            });
    });

Task("BuildForTests")
    .Does(() =>
    {
        DotNetCoreBuild(
            "../ShoppingCart.sln",
            new DotNetCoreBuildSettings
            {
                Configuration = configuration,
                NoRestore = true,
            });
    });

Task("UnitTests")
    .Does(() =>
    {
        var testSettings = TestSettings;
        testSettings.Filter = "TestCategory=Unit";
        DotNetCoreTest(
            "../test/Tests/Tests.csproj",
        testSettings);
    });

Task("Coverage")
    .Does(() =>
    {
        var outputFolder = "../coverage";
        var reportFile = new FilePath($"{outputFolder}/coverage.xml");
        EnsureDirectoryExists(outputFolder);
        OpenCover(tool =>
        {
            tool.DotNetCoreTest(
                "../ShoppingCart.sln",
                TestSettings);
        },
        reportFile,
        new OpenCoverSettings()
        {
            ArgumentCustomization = args => args.Append("-oldstyle"),
        }
        .WithFilter("+[ShoppingCart*]*")
        .WithFilter("-[*Tests*]*")
        .ExcludeByAttribute("*.ExcludeFromCodeCoverageAttribute"));
        //ReportCoverageMetrics(reportFile.FullPath);
        try
        {
            ReportGenerator(reportFile, outputFolder);
        }
        catch (Exception e)
        {
            Information("Generating coverage report failed.");
            Information(e);
        }
    });

void ReportCoverageMetrics(string openCoverResultsXmlFile)
{
    var doc = System.Xml.Linq.XDocument.Load(openCoverResultsXmlFile);
    var summary = doc.XPathSelectElement("/CoverageSession/Summary");

    // Classes.
    ReportCoverageMetric(summary,
        "visitedClasses", "CodeCoverageAbsCCovered",
        "numClasses", "CodeCoverageAbsCTotal",
        "CodeCoverageC");

    // Methods.
    ReportCoverageMetric(summary,
        "visitedMethods", "CodeCoverageAbsMCovered",
        "numMethods", "CodeCoverageAbsMTotal",
        "CodeCoverageM");

    // Sequence points / statements.
    ReportCoverageMetric(summary,
        "visitedSequencePoints", "CodeCoverageAbsSCovered",
        "numSequencePoints", "CodeCoverageAbsSTotal",
        "CodeCoverageS");

    // Branches.
    ReportCoverageMetric(summary,
        "visitedBranchPoints", "CodeCoverageAbsBCovered",
        "numBranchPoints", "CodeCoverageAbsBTotal",
        "CodeCoverageB");
}

void ReportCoverageMetric(
    System.Xml.Linq.XElement summary,
    string ocVisitedAttr,
    string tcVisitedKey,
    string ocTotalAttr,
    string tcTotalKey,
    string tcCoverageKey)
{
    double visited = Convert.ToDouble(summary.Attribute(ocVisitedAttr).Value);
    double total = Convert.ToDouble(summary.Attribute(ocTotalAttr).Value);
    double coverage = (visited / total) * 100;

    Information($"##teamcity[buildStatisticValue key='{tcVisitedKey}' value='{visited}']");
    Information($"##teamcity[buildStatisticValue key='{tcTotalKey}' value='{total}']");
    Information($"##teamcity[buildStatisticValue key='{tcCoverageKey}' value='{coverage}']");
}


Task("UpdateVersions")
    .Does(() =>
    {
        Information("Updating version number in *csproj files.");
        ReplaceRegexInFiles(
            "../src/**/*.csproj",
            @"(<(Version)\s*>).*(<\/\s*\2\s*>)",
            "<Version>"+fullVersion+"</Version>");
    });

Task("BuildWebApi")
    .Does(() =>
    {
        DotNetCorePublish(
            "../src/WebApi/WebApi.csproj",
            new DotNetCorePublishSettings
            {
                Framework = "netcoreapp2.2",
                Configuration = configuration,
                Runtime = runtime,
                OutputDirectory = "../publish/WebApi",
                SelfContained = true,
            });
        DeleteFile("../publish/WebApi/appsettings.Development.json");
    });

Task("BuildHttpClients")
    .IsDependentOn("UpdateVersions")
    .Does(() =>
    {
        DotNetCorePack(
            "../src/HttpClients/HttpClients.csproj",
            new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory = "../publish/HttpClients",
                VersionSuffix = revisionSuffix,
            });
    });

Task("Test")
    .IsDependentOn("BuildForTests")
    //.IsDependentOn("UnitTests")
    .IsDependentOn("Coverage");

Task("Build")
    .IsDependentOn("UpdateVersions")
    .IsDependentOn("BuildWebApi")
    .IsDependentOn("BuildHttpClients")
    .Finally(() =>
    {
        try
        {
            Information("Reverting version number change in *.csproj files.");
            ReplaceRegexInFiles(
                "../src/**/*.csproj",
                @"(<(Version)\s*>).*(<\/\s*\2\s*>)",
                "<Version>1.0.0</Version>");
        }
        catch
        {
            // Ignore
            Information("Reverting version number in *.csproj files failed.");
        }
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Test")
    .IsDependentOn("Build");

var target = Argument("target", "Default");

RunTarget(target);