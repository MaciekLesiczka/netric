@echo off
::".nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "packages" "-ExcludeVersion"
"src\packages\FAKE\tools\Fake.exe" build.fsx