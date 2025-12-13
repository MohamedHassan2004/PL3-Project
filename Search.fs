/// Search module - Case-insensitive and partial match search
namespace DictionaryApp

/// Module containing search operations
module Search =
    
    /// Case-insensitive exact match search
    /// Returns matching entries as (word, meaning) pairs
    let searchExact (query: string) (direction: TranslationDirection) (dict: BilingualDictionary) 
        : (string * string) list =
        let currentMap = Dictionary.getMap direction dict
        let queryLower = query.Trim().ToLowerInvariant()
        
        if System.String.IsNullOrWhiteSpace(queryLower) then
            []
        else
            currentMap
            |> Map.toList
            |> List.filter (fun (key, _) -> key.ToLowerInvariant() = queryLower)
    
    /// Case-insensitive partial/substring match search
    /// Returns matching entries as (word, meaning) pairs
    let searchPartial (query: string) (direction: TranslationDirection) (dict: BilingualDictionary) 
        : (string * string) list =
        let currentMap = Dictionary.getMap direction dict
        let queryLower = query.Trim().ToLowerInvariant()
        
        if System.String.IsNullOrWhiteSpace(queryLower) then
            []
        else
            currentMap
            |> Map.toList
            |> List.filter (fun (key, value) -> 
                key.ToLowerInvariant().Contains(queryLower) || 
                value.ToLowerInvariant().Contains(queryLower))
    
    /// Search in both word and meaning with partial matching
    let searchAll (query: string) (direction: TranslationDirection) (dict: BilingualDictionary) 
        : (string * string) list =
        if System.String.IsNullOrWhiteSpace(query) then
            Crud.getAllEntries direction dict
        else
            searchPartial query direction dict
