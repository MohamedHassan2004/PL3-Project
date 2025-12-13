/// CRUD module - Create, Read, Update, Delete operations for dictionary
namespace DictionaryApp

/// Module containing all CRUD operations
/// All operations return new immutable Maps
module Crud =
    
    /// Adds a new word to the dictionary
    /// Returns Error if word already exists
    let addWord (word: string) (meaning: string) (direction: TranslationDirection) (dict: BilingualDictionary) 
        : Result<BilingualDictionary, DictionaryError> =
        let currentMap = Dictionary.getMap direction dict
        let key = word.Trim()
        
        if System.String.IsNullOrWhiteSpace(key) then
            Error (ValidationError "Word cannot be empty")
        elif currentMap.ContainsKey(key) then
            Error (WordAlreadyExists key)
        else
            let newMap = currentMap.Add(key, meaning.Trim())
            Ok (Dictionary.setMap direction newMap dict)
    
    /// Updates an existing word in the dictionary
    /// Returns Error if word does not exist
    let updateWord (word: string) (newMeaning: string) (direction: TranslationDirection) (dict: BilingualDictionary) 
        : Result<BilingualDictionary, DictionaryError> =
        let currentMap = Dictionary.getMap direction dict
        let key = word.Trim()
        
        if System.String.IsNullOrWhiteSpace(key) then
            Error (ValidationError "Word cannot be empty")
        elif not (currentMap.ContainsKey(key)) then
            Error (WordNotFound key)
        else
            let newMap = currentMap.Add(key, newMeaning.Trim())
            Ok (Dictionary.setMap direction newMap dict)
    
    /// Deletes a word from the dictionary
    /// Returns Error if word does not exist
    let deleteWord (word: string) (direction: TranslationDirection) (dict: BilingualDictionary) 
        : Result<BilingualDictionary, DictionaryError> =
        let currentMap = Dictionary.getMap direction dict
        let key = word.Trim()
        
        if System.String.IsNullOrWhiteSpace(key) then
            Error (ValidationError "Word cannot be empty")
        elif not (currentMap.ContainsKey(key)) then
            Error (WordNotFound key)
        else
            let newMap = currentMap.Remove(key)
            Ok (Dictionary.setMap direction newMap dict)
    
    /// Gets the meaning of a word
    /// Returns None if word does not exist
    let getWord (word: string) (direction: TranslationDirection) (dict: BilingualDictionary) : string option =
        let currentMap = Dictionary.getMap direction dict
        currentMap.TryFind(word.Trim())
    
    /// Gets all entries in the dictionary for a direction
    let getAllEntries (direction: TranslationDirection) (dict: BilingualDictionary) : (string * string) list =
        let currentMap = Dictionary.getMap direction dict
        currentMap |> Map.toList
