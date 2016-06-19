@echo off

if not exist "src\packages\FAKE\tools\Fake.exe" (
    "src\.nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "src\packages" "-ExcludeVersion" "-Prerelease"
)

"src\packages\FAKE\tools\Fake.exe" "build.fsx"