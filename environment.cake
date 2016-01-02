#l "common.cake"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var baseDir=System.IO.Directory.GetCurrentDirectory();
var cassandraMsiUrl = "http://downloads.datastax.com/community/datastax-community-64bit_2.2.4.msi";
var cassandraMsiLocation = System.IO.Path.Combine(baseDir, "tools", "cassandra.msi");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("DownloadCassandra")
    .Does(() =>
{
    if(System.IO.File.Exists(cassandraMsiLocation))
    {
        Information("Cassandra is already downloaded...");
        return;
    }

    DownloadFile(cassandraMsiUrl, cassandraMsiLocation);
});

Task("InstallCassandra")
    .IsDependentOn("DownloadCassandra")
    .Does(() =>
{
    ExecuteCommand("start /wait msiexec /i \"" + cassandraMsiLocation + "\" /passive /qb /l*v \"" +  System.IO.Path.Combine(baseDir, "tools", "cassandra.log") + "\"");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("DownloadCassandra")
    .IsDependentOn("InstallCassandra");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
