#tool "nuget:?package=OpenCover"
#tool "nuget:?package=xunit.runner.console"

// ARGUMENTS

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");


Task("Default")
  .IsDependentOn("Artifacts");

Task("Build")
  .IsDependentOn("Restore-NuGet-Packages")
  .Does(() =>
{
  MSBuild("./src/DevOpsAssignment.sln");
});

Task("Clean")
  .Does(() =>
  {
    CleanDirectories(string.Format("./src/**/obj/{0}",
      configuration));
    CleanDirectories(string.Format("./src/**/bin/{0}",
      configuration));
  });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/DevOpsAssignment.sln");
});

Task("xUnit")
  .IsDependentOn("Build")
    .Does(() =>
{
  XUnit2("./src/DevOpsAssignment.Tests/bin/Debug/DevOpsAssignment.Tests.dll");
});

Task("Coverage")
	.IsDependentOn("xUnit")
	.Does(() =>
	{
		OpenCover(tool => {
		  tool.XUnit2("./src/DevOpsAssignment.Tests/bin/Debug/DevOpsAssignment.Tests.dll",
        new XUnit2Settings {
          ShadowCopy = false
        });
		  },
		  new FilePath("./coverage.xml"),
		  new OpenCoverSettings()
			.WithFilter("+[DevOpsAssignment]*")
      .WithFilter("-[DevOpsAssignment.Tests]*"));
	});
	
Task("Artifacts")
	.IsDependentOn("Coverage")
	.Does(() =>
	{
		MSBuild("./src/DevOpsAssignment/DevOpsAssignment.csproj", new MSBuildSettings()
		  .WithProperty("DeployOnBuild", "true")
		  .WithProperty("WebPublishMethod", "Package")
		  .WithProperty("PackageAsSingleFile", "true")
		  .WithProperty("SkipInvalidConfigurations", "true"));
	});


RunTarget(target);