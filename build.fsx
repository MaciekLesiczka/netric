#r @"src/packages/FAKE/tools/fakelib.dll"

open Fake
let buildDir = "./build/"
let testDir = "./test/"

let appProjects = 
    !! "src/**/Netric.Agent.Service.csproj"
    ++ "src/**/Netric.Configuration.Console.csproj"
let testProjects = !! "src/**/*.Tests.csproj"
let version = "1.0.0.0"

Target "Clean" (fun _ ->
    CleanDir buildDir
    CleanDir testDir
)

Target "BuildApp" (fun _ ->    
    appProjects      
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
    !! "src/Netric.Profiler*.dll"
      |> CopyFiles buildDir
)

Target "BuildTest" (fun _ ->
    MSBuildDebug testDir "Build" testProjects
        |> Log "TestBuild-Output: "
)

open Fake.Testing
Target "RunTests" (fun _ ->  
    !! (testDir + "/*.Tests.dll")
        |> xUnit2 (fun p -> 
            {p with 
               ToolPath = "src/packages/xunit.runner.console.2.1.0/tools/xunit.console.exe"
               HtmlOutputPath = Some(testDir + "result.html")
               })
)

Target "Default" (fun _ -> 
    trace "Staring full build"
)

"Clean" 
    ==> "BuildApp"  
    ==> "BuildTest"
    ==> "RunTests"
    ==> "Default"

RunTargetOrDefault "Default"