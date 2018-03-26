#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Git
open Fake.Testing.XUnit2

let buildDir = "./build/"
let testProjects = "./source/*Tests/*.csproj"
let testOutputDir = "./tests/"
let projectReferences = "./source/Halite.Serialization.JsonNet/Halite.Serialization.JsonNet.csproj" 

let testProjectReferences = !! "./source/Halite.Tests/Halite.Tests.csproj"
let projectName = "Halite.Serialization.JsonNet"
let description = "JSON serialization support for HAL objects and links."
let version = environVarOrDefault "version" "0.0.0"
let assemblyGuid = "670c2953-95c3-493c-a39c-987105130378"
let commitHash = Information.getCurrentSHA1(".")
let templateFilePath = "./Halite.Serialization.JsonNet.paket.template"
let toolPathPaket = ".paket/paket.exe"

let nugetSources = (environVarOrDefault "nuget.sources" "https://api.nuget.org/v3/index.json,https://www.myget.org/F/nrk/auth/c64eb6ac-a674-493a-9099-320dee35da47/api/v3/index.json").Split([|','|]) 
                    |> Array.toList
                    |> List.map (fun source -> "-s " + source)

Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p ->
        {p with
            AdditionalArgs = nugetSources
            Project = "./source/Halite.Serialization.JsonNet/Halite.Serialization.JsonNet.csproj" })   
)

Target "Clean" (fun _ ->
  CleanDirs [buildDir; testOutputDir]
)

Target "AddAssemblyVersion" (fun _ -> 
    let assemblyInfos = !!(@"../**/AssemblyInfo.cs") 

    ReplaceAssemblyInfoVersionsBulk assemblyInfos (fun f -> 
        { f with 
            AssemblyFileVersion = version
            AssemblyVersion = version })  
)

Target "Build" (fun _ -> 
    DotNetCli.Build (fun p -> 
        { p with
            Output = "../../" + buildDir
            Configuration = "Release"
            Project = projectReferences }) 
)

Target "BuildTests" (fun _ ->  MSBuild testOutputDir "Build" [ "Configuration", "Debug" ] testProjectReferences |> Log "TestBuild-Output: ")

Target "RunTests" (fun _ ->
  !! (testOutputDir @@ "*Tests.dll")
  |> xUnit2 (fun p ->
                 { p with HtmlOutputPath = Some (testOutputDir @@ "xunit.html") })
)

Target "CreateNugetPackage" (fun _ -> 
    DotNetCli.Pack (fun c -> 
        { c with
            Configuration = "Release"
            Project = projectReferences
            AdditionalArgs = [ 
                                "/p:PackageVersion=" + version
                                "/p:Version=" + version]
            OutputPath = "../../" + buildDir
        }
    )
)

Target "PushPackage" (fun _ ->
  Paket.Push (fun p -> 
      {
          p with
            ApiKey = environVarOrDefault "NUGET_KEY" ""
            PublishUrl = "https://www.nuget.org/api/v2/package"
            ToolPath = toolPathPaket
            WorkingDir = "./build/"
      })
)

"Clean"
==> "AddAssemblyVersion"
==> "Restore"
==> "Build"
==> "BuildTests"
==> "RunTests"
==> "CreateNugetPackage"
==> "PushPackage"

RunTargetOrDefault "CreateNugetPackage"
