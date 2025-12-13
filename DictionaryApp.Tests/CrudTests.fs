/// CRUD Operations Unit Tests
module DictionaryApp.Tests.CrudTests

open Xunit
open FsUnit.Xunit
open DictionaryApp

[<Fact>]
let ``addWord should add new word to empty dictionary`` () =
    let result = Crud.addWord "hello" "مرحبا" EnToAr Dictionary.empty
    
    match result with
    | Ok dict ->
        dict.EnToAr |> Map.containsKey "hello" |> should equal true
        dict.EnToAr.["hello"] |> should equal "مرحبا"
    | Error _ -> failwith "Expected Ok result"

[<Fact>]
let ``addWord should return error for duplicate word`` () =
    let dict = { Dictionary.empty with EnToAr = Map.ofList [("hello", "مرحبا")] }
    let result = Crud.addWord "hello" "different" EnToAr dict
    
    match result with
    | Ok _ -> failwith "Expected Error result"
    | Error (WordAlreadyExists word) -> word |> should equal "hello"
    | Error _ -> failwith "Expected WordAlreadyExists error"

[<Fact>]
let ``addWord should return error for empty word`` () =
    let result = Crud.addWord "" "meaning" EnToAr Dictionary.empty
    
    match result with
    | Ok _ -> failwith "Expected Error result"
    | Error (ValidationError _) -> ()
    | Error _ -> failwith "Expected ValidationError"

[<Fact>]
let ``addWord should return new Map instance`` () =
    let original = Dictionary.empty
    let result = Crud.addWord "test" "تجربة" EnToAr original
    
    match result with
    | Ok newDict ->
        original.EnToAr |> Map.isEmpty |> should equal true
        newDict.EnToAr |> Map.isEmpty |> should equal false
    | Error _ -> failwith "Expected Ok result"

[<Fact>]
let ``updateWord should update existing word`` () =
    let dict = { Dictionary.empty with ArToEn = Map.ofList [("كتاب", "Book")] }
    let result = Crud.updateWord "كتاب" "A Book" ArToEn dict
    
    match result with
    | Ok newDict ->
        newDict.ArToEn.["كتاب"] |> should equal "A Book"
    | Error _ -> failwith "Expected Ok result"

[<Fact>]
let ``updateWord should return error for non-existent word`` () =
    let result = Crud.updateWord "nonexistent" "meaning" EnToAr Dictionary.empty
    
    match result with
    | Ok _ -> failwith "Expected Error result"
    | Error (WordNotFound word) -> word |> should equal "nonexistent"
    | Error _ -> failwith "Expected WordNotFound error"

[<Fact>]
let ``deleteWord should remove existing word`` () =
    let dict = { Dictionary.empty with EnToAr = Map.ofList [("hello", "مرحبا")] }
    let result = Crud.deleteWord "hello" EnToAr dict
    
    match result with
    | Ok newDict ->
        newDict.EnToAr |> Map.containsKey "hello" |> should equal false
    | Error _ -> failwith "Expected Ok result"

[<Fact>]
let ``deleteWord should return error for non-existent word`` () =
    let result = Crud.deleteWord "nonexistent" EnToAr Dictionary.empty
    
    match result with
    | Ok _ -> failwith "Expected Error result"
    | Error (WordNotFound word) -> word |> should equal "nonexistent"
    | Error _ -> failwith "Expected WordNotFound error"

[<Fact>]
let ``getWord should return meaning for existing word`` () =
    let dict = { Dictionary.empty with ArToEn = Map.ofList [("شمس", "Sun")] }
    let result = Crud.getWord "شمس" ArToEn dict
    
    result |> should equal (Some "Sun")

[<Fact>]
let ``getWord should return None for non-existent word`` () =
    let result = Crud.getWord "nonexistent" ArToEn Dictionary.empty
    
    result |> should equal None

[<Fact>]
let ``getAllEntries should return all entries`` () =
    let dict = { Dictionary.empty with 
                    EnToAr = Map.ofList [("Hello", "مرحبا"); ("Book", "كتاب")] }
    let entries = Crud.getAllEntries EnToAr dict
    
    entries |> List.length |> should equal 2
