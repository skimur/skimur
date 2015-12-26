@echo off
cd %~dp0

SETLOCAL
SET NUGET_VERSION=latest
SET CACHED_NUGET=%LocalAppData%\NuGet\nuget.%NUGET_VERSION%.exe
SET BUILDCMD_SAKEEXTENSIONS_VERSION=

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/%NUGET_VERSION%/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul

:restore
IF EXIST packages\Sake goto run
IF "%BUILDCMD_SAKEEXTENSIONS_VERSION%"=="" (
    .nuget\nuget.exe install SakeExtensions -ExcludeVersion -o packages -nocache -pre
) ELSE (
    .nuget\nuget.exe install SakeExtensions -version %BUILDCMD_SAKEEXTENSIONS_VERSION% -ExcludeVersion -o packages -nocache -pre
)
.nuget\NuGet.exe install Sake -ExcludeVersion -Source https://www.nuget.org/api/v2/ -Out packages

:run
packages\Sake\tools\Sake.exe -I packages\SakeExtensions\build -f makefile.shade %*
