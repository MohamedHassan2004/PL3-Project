/// File I/O module - JSON serialization and persistence
namespace DictionaryApp

open System
open System.IO
open System.Text.Json

/// DTO for JSON serialization
type DictionaryDto = {
    ArToEn: System.Collections.Generic.Dictionary<string, string>
    EnToAr: System.Collections.Generic.Dictionary<string, string>
}

/// Module for JSON file operations
module FileIO =
    
    let private jsonOptions = 
        let opts = JsonSerializerOptions()
        opts.WriteIndented <- true
        opts.PropertyNameCaseInsensitive <- true
        opts
    
    /// Converts BilingualDictionary to DTO for serialization
    let private toDto (dict: BilingualDictionary) : DictionaryDto =
        let arToEnDict = System.Collections.Generic.Dictionary<string, string>()
        let enToArDict = System.Collections.Generic.Dictionary<string, string>()
        
        dict.ArToEn |> Map.iter (fun k v -> arToEnDict.Add(k, v))
        dict.EnToAr |> Map.iter (fun k v -> enToArDict.Add(k, v))
        
        { ArToEn = arToEnDict; EnToAr = enToArDict }
    
    /// Converts DTO back to BilingualDictionary
    let private fromDto (dto: DictionaryDto) : BilingualDictionary =
        let arToEn = 
            if isNull dto.ArToEn then Map.empty
            else dto.ArToEn |> Seq.map (fun kv -> kv.Key, kv.Value) |> Map.ofSeq
        
        let enToAr = 
            if isNull dto.EnToAr then Map.empty
            else dto.EnToAr |> Seq.map (fun kv -> kv.Key, kv.Value) |> Map.ofSeq
        
        { ArToEn = arToEn; EnToAr = enToAr }
    
    /// Saves dictionary to JSON file
    let saveToJson (filePath: string) (dict: BilingualDictionary) : Result<unit, DictionaryError> =
        try
            let dto = toDto dict
            let json = JsonSerializer.Serialize(dto, jsonOptions)
            File.WriteAllText(filePath, json)
            Ok ()
        with
        | ex -> Error (FileWriteError (filePath, ex.Message))
    
    /// Loads dictionary from JSON file
    let loadFromJson (filePath: string) : Result<BilingualDictionary, DictionaryError> =
        try
            if not (File.Exists(filePath)) then
                Error (FileNotFound filePath)
            else
                let json = File.ReadAllText(filePath)
                let dto = JsonSerializer.Deserialize<DictionaryDto>(json, jsonOptions)
                Ok (fromDto dto)
        with
        | :? JsonException as ex -> 
            Error (JsonParseError ex.Message)
        | ex -> 
            Error (FileReadError (filePath, ex.Message))
