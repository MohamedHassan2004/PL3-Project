/// Types module - Core domain types for the Dictionary Application
namespace DictionaryApp

/// Translation direction for bilingual dictionary
type TranslationDirection =
    | ArToEn  // Arabic to English
    | EnToAr  // English to Arabic

/// Bilingual dictionary containing both translation directions
type BilingualDictionary = {
    /// Arabic to English translations
    ArToEn: Map<string, string>
    /// English to Arabic translations
    EnToAr: Map<string, string>
}

/// Module for working with bilingual dictionaries
module Dictionary =
    /// Creates an empty bilingual dictionary
    let empty: BilingualDictionary = {
        ArToEn = Map.empty
        EnToAr = Map.empty
    }
    
    /// Gets the appropriate map based on translation direction
    let getMap (direction: TranslationDirection) (dict: BilingualDictionary) =
        match direction with
        | ArToEn -> dict.ArToEn
        | EnToAr -> dict.EnToAr
    
    /// Updates the appropriate map based on translation direction
    let setMap (direction: TranslationDirection) (map: Map<string, string>) (dict: BilingualDictionary) =
        match direction with
        | ArToEn -> { dict with ArToEn = map }
        | EnToAr -> { dict with EnToAr = map }
