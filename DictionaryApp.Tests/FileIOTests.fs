/// File I/O Unit Tests
module DictionaryApp.Tests.FileIOTests

open System
open System.IO
open Xunit
open FsUnit.Xunit
open DictionaryApp

let sampleDict = { 
    ArToEn = Map.ofList [("مرحبا", "Hello"); ("كتاب", "Book")]
    EnToAr = Map.ofList [("Hello", "مرحبا"); ("Book", "كتاب")]
}

[<Fact>]
let ``saveToJson and loadFromJson should round-trip dictionary`` () =
    let tempFile = Path.Combine(Path.GetTempPath(), sprintf "dict_test_%s.json" (Guid.NewGuid().ToString()))
    
    try
        // Save
        let saveResult = FileIO.saveToJson tempFile sampleDict
        match saveResult with
        | Ok () -> ()
        | Error err -> failwith (Error.toMessage err)
        
        // Load
        let loadResult = FileIO.loadFromJson tempFile
        
        match loadResult with
        | Ok loadedDict ->
            loadedDict.ArToEn |> Map.count |> should equal 2
            loadedDict.EnToAr |> Map.count |> should equal 2
            loadedDict.ArToEn.["مرحبا"] |> should equal "Hello"
            loadedDict.EnToAr.["Hello"] |> should equal "مرحبا"
        | Error err -> failwith (Error.toMessage err)
    finally
        if File.Exists(tempFile) then File.Delete(tempFile)

[<Fact>]
let ``loadFromJson should return FileNotFound for missing file`` () =
    let result = FileIO.loadFromJson "C:\\nonexistent\\path\\file.json"
    
    match result with
    | Ok _ -> failwith "Expected Error result"
    | Error (FileNotFound _) -> ()
    | Error err -> failwith (sprintf "Expected FileNotFound, got: %s" (Error.toMessage err))

[<Fact>]
let ``loadFromJson should return JsonParseError for invalid JSON`` () =
    let tempFile = Path.Combine(Path.GetTempPath(), sprintf "invalid_%s.json" (Guid.NewGuid().ToString()))
    
    try
        File.WriteAllText(tempFile, "{ invalid json }")
        let result = FileIO.loadFromJson tempFile
        
        match result with
        | Ok _ -> failwith "Expected Error result"
        | Error (JsonParseError _) -> ()
        | Error err -> failwith (sprintf "Expected JsonParseError, got: %s" (Error.toMessage err))
    finally
        if File.Exists(tempFile) then File.Delete(tempFile)

[<Fact>]
let ``saveToJson should create valid JSON file`` () =
    let tempFile = Path.Combine(Path.GetTempPath(), sprintf "valid_%s.json" (Guid.NewGuid().ToString()))
    
    try
        let result = FileIO.saveToJson tempFile sampleDict
        match result with
        | Ok () -> ()
        | Error err -> failwith (Error.toMessage err)
        
        let content = File.ReadAllText(tempFile)
        Assert.True(content.Contains("ArToEn"), "Should contain ArToEn")
        Assert.True(content.Contains("EnToAr"), "Should contain EnToAr")
        Assert.True(content.Contains("Hello"), "Should contain Hello")
    finally
        if File.Exists(tempFile) then File.Delete(tempFile)

[<Fact>]
let ``loadFromJson should handle empty dictionary`` () =
    let tempFile = Path.Combine(Path.GetTempPath(), sprintf "empty_%s.json" (Guid.NewGuid().ToString()))
    
    try
        let emptyDict = Dictionary.empty
        FileIO.saveToJson tempFile emptyDict |> ignore
        
        let result = FileIO.loadFromJson tempFile
        
        match result with
        | Ok loadedDict ->
            loadedDict.ArToEn |> Map.isEmpty |> should equal true
            loadedDict.EnToAr |> Map.isEmpty |> should equal true
        | Error err -> failwith (Error.toMessage err)
    finally
        if File.Exists(tempFile) then File.Delete(tempFile)
