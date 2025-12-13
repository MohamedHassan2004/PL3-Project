/// Error handling module - Unified error types and Result helpers
namespace DictionaryApp

/// All possible errors in the dictionary application
type DictionaryError =
    | WordAlreadyExists of word: string
    | WordNotFound of word: string
    | FileNotFound of path: string
    | FileReadError of path: string * message: string
    | FileWriteError of path: string * message: string
    | JsonParseError of message: string
    | ValidationError of message: string

/// Module for error handling utilities
module Error =
    /// Converts a DictionaryError to a user-friendly message
    let toMessage (error: DictionaryError) : string =
        match error with
        | WordAlreadyExists word -> sprintf "Word '%s' already exists in the dictionary." word
        | WordNotFound word -> sprintf "Word '%s' was not found in the dictionary." word
        | FileNotFound path -> sprintf "File not found: %s" path
        | FileReadError (path, msg) -> sprintf "Error reading file '%s': %s" path msg
        | FileWriteError (path, msg) -> sprintf "Error writing file '%s': %s" path msg
        | JsonParseError msg -> sprintf "JSON parsing error: %s" msg
        | ValidationError msg -> sprintf "Validation error: %s" msg
    
    /// Binds a Result, applying the function if Ok
    let bind (f: 'a -> Result<'b, 'e>) (result: Result<'a, 'e>) : Result<'b, 'e> =
        match result with
        | Ok value -> f value
        | Error e -> Error e
    
    /// Maps a Result value if Ok
    let map (f: 'a -> 'b) (result: Result<'a, 'e>) : Result<'b, 'e> =
        match result with
        | Ok value -> Ok (f value)
        | Error e -> Error e
