/// Search Engine Unit Tests
module DictionaryApp.Tests.SearchTests

open Xunit
open FsUnit.Xunit
open DictionaryApp

let sampleDict = { 
    Dictionary.empty with 
        EnToAr = Map.ofList [
            ("Hello", "مرحبا")
            ("HELLO", "مرحبا مرة أخرى")
            ("Book", "كتاب")
            ("Bookstore", "مكتبة")
            ("Notebook", "دفتر")
        ]
        ArToEn = Map.ofList [
            ("مرحبا", "Hello")
            ("كتاب", "Book")
        ]
}

[<Fact>]
let ``searchExact should find case-insensitive match`` () =
    let results = Search.searchExact "hello" EnToAr sampleDict
    
    results |> List.length |> should equal 2

[<Fact>]
let ``searchExact should return empty for no match`` () =
    let results = Search.searchExact "nonexistent" EnToAr sampleDict
    
    results |> List.length |> should equal 0

[<Fact>]
let ``searchExact should return empty for empty query`` () =
    let results = Search.searchExact "" EnToAr sampleDict
    
    results |> List.length |> should equal 0

[<Fact>]
let ``searchExact should return empty for whitespace query`` () =
    let results = Search.searchExact "   " EnToAr sampleDict
    
    results |> List.length |> should equal 0

[<Fact>]
let ``searchPartial should find substring matches in keys`` () =
    let results = Search.searchPartial "book" EnToAr sampleDict
    
    results |> List.length |> should equal 3 // Book, Bookstore, Notebook

[<Fact>]
let ``searchPartial should find matches in values`` () =
    let results = Search.searchPartial "مرحبا" EnToAr sampleDict
    
    results |> List.length |> should equal 2 // Both entries with مرحبا in meaning

[<Fact>]
let ``searchPartial should be case-insensitive`` () =
    let results = Search.searchPartial "BOOK" EnToAr sampleDict
    
    results |> List.length |> should equal 3

[<Fact>]
let ``searchAll should return all entries for empty query`` () =
    let results = Search.searchAll "" EnToAr sampleDict
    
    results |> List.length |> should equal 5

[<Fact>]
let ``searchAll should filter for non-empty query`` () =
    let results = Search.searchAll "note" EnToAr sampleDict
    
    results |> List.length |> should equal 1
    results |> List.head |> fst |> should equal "Notebook"

[<Fact>]
let ``search should work with Arabic direction`` () =
    let results = Search.searchPartial "كتاب" ArToEn sampleDict
    
    results |> List.length |> should equal 1
    results |> List.head |> snd |> should equal "Book"
