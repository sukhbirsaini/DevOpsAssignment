#tool "nuget:?package=xunit.runner.console"


// ARGUMENTS

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");


Task("Default")
  .IsDependentOn("xUnit");

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

RunTarget(target);