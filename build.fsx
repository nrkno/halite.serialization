#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Git
open Fake.Testing.XUnit2
open Fake.PaketTemplate

let buildDir = "./build/"
let testProjects = "./source/*Tests/*.csproj"
let testOutputDir = "./tests/"
let projectReferences = !! "./source/Halite.Serialization.JsonNet/Halite.Serialization.JsonNet.csproj" 
                        ++ "./source/Halite.Examples/Halite.Examples.csproj" 

let testProjectReferences = !! "./source/Halite.Tests/Halite.Tests.csproj"
                            ++ "./source/Halite.Examples.Tests/Halite.Examples.Tests.csproj"
let projectName = "Halite.Serialization.JsonNet"
let description = "JSON serialization support for HAL objects and links."
let version = environVarOrDefault "version" "0.0.0"
let assemblyGuid = "670c2953-95c3-493c-a39c-987105130378"
let commitHash = Information.getCurrentSHA1(".")
let templateFilePath = "./Halite.Serialization.JsonNet.paket.template"
let toolPathPaket = ".paket/paket.exe"

Target "Clean" (fun _ ->
  CleanDirs [buildDir; testOutputDir]
)

let buildReleaseProperties = 
  [ "Configuration", "Release"
    "DocumentationFile", "Halite.Serialization.JsonNet.xml" ]

Target "AddAssemblyVersion" (fun _ -> 
    let assemblyInfos = !!(@"../**/AssemblyInfo.cs") 

    ReplaceAssemblyInfoVersionsBulk assemblyInfos (fun f -> 
        { f with 
            AssemblyFileVersion = version
            AssemblyVersion = version })  
)

Target "Build" (fun _ -> MSBuild buildDir "Build" buildReleaseProperties projectReferences |> Log "Building project: ")

Target "BuildTests" (fun _ ->  MSBuild testOutputDir "Build" [ "Configuration", "Debug" ] testProjectReferences |> Log "TestBuild-Output: ")

Target "RunTests" (fun _ ->
  !! (testOutputDir @@ "*Tests.dll")
  |> xUnit2 (fun p ->
                 { p with HtmlOutputPath = Some (testOutputDir @@ "xunit.html") })
)

Target "CreatePaketTemplate" (fun _ ->
  PaketTemplate (fun p ->
    {
        p with
          TemplateFilePath = Some templateFilePath
          TemplateType = File
          Description = ["Support for serialization of Halite objects using Json.NET."]
          Id = Some projectName
          Version = Some version
          Authors = ["NRK"]
          Files = [ Include (buildDir + "Halite.Serialization.JsonNet.dll", "lib/net45")
                    Include (buildDir + "Halite.Serialization.JsonNet.pdb", "lib/net45")
                    Include (buildDir + "Halite.Serialization.JsonNet.xml", "lib/net45") ]
          Dependencies = 
            [ "Halite", GreaterOrEqual (Version "1.2.0")
              "Newtonsoft.Json", GreaterOrEqual (Version "6.0.8") 
              "JetBrains.Annotations", GreaterOrEqual (Version "11.1.0") ]
    } )
)

Target "CreatePackage" (fun _ ->
    Paket.Pack (fun p ->
      {
          p with
              Version = version
              ReleaseNotes = "fake release"
              OutputPath = buildDir
              TemplateFile = templateFilePath
              BuildConfig = "Release"
              ToolPath = toolPathPaket })
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
==> "Build"
==> "BuildTests"
==> "RunTests"
==> "CreatePaketTemplate"
==> "CreatePackage"
==> "PushPackage"

RunTargetOrDefault "CreatePackage"
