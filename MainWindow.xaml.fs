/// MainWindow code-behind - UI event handlers and state management
namespace DictionaryApp

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup
open System.IO

/// Main window for the Dictionary Application
type MainWindow() as this =
    inherit Window()
    
    // Mutable reference to immutable dictionary state
    let mutable currentDictionary = Dictionary.empty
    
    // UI Elements (initialized after XAML loads)
    let mutable directionComboBox: ComboBox = null
    let mutable wordTextBox: TextBox = null
    let mutable meaningTextBox: TextBox = null
    let mutable resultsListBox: ListBox = null
    let mutable statusText: TextBlock = null
    
    // Initialize window from XAML
    do
        let xamlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MainWindow.xaml")
        
        // Try to load from file first, then from embedded resource
        let xamlContent = 
            if File.Exists(xamlPath) then
                File.ReadAllText(xamlPath)
            else
                let assemblyPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "MainWindow.xaml")
                if File.Exists(assemblyPath) then
                    File.ReadAllText(assemblyPath)
                else
                    // Fallback: look relative to current directory
                    let currentDirPath = Path.Combine(Environment.CurrentDirectory, "MainWindow.xaml")
                    if File.Exists(currentDirPath) then
                        File.ReadAllText(currentDirPath)
                    else
                        failwith (sprintf "Could not find MainWindow.xaml. Searched: %s, %s" xamlPath (Environment.CurrentDirectory))
        
        // Parse XAML and store reference
        let parsedWindow = XamlReader.Parse(xamlContent) :?> Window
        
        // Copy properties from parsed window
        this.Title <- parsedWindow.Title
        this.Height <- parsedWindow.Height
        this.Width <- parsedWindow.Width
        this.WindowStartupLocation <- parsedWindow.WindowStartupLocation
        this.Background <- parsedWindow.Background
        this.Content <- parsedWindow.Content
        this.Resources <- parsedWindow.Resources
        
        // Get UI elements from parsed window's namescope
        directionComboBox <- parsedWindow.FindName("DirectionComboBox") :?> ComboBox
        wordTextBox <- parsedWindow.FindName("WordTextBox") :?> TextBox
        meaningTextBox <- parsedWindow.FindName("MeaningTextBox") :?> TextBox
        resultsListBox <- parsedWindow.FindName("ResultsListBox") :?> ListBox
        statusText <- parsedWindow.FindName("StatusText") :?> TextBlock
        
        // Wire up event handlers
        let addButton = parsedWindow.FindName("AddButton") :?> Button
        let updateButton = parsedWindow.FindName("UpdateButton") :?> Button
        let deleteButton = parsedWindow.FindName("DeleteButton") :?> Button
        let searchButton = parsedWindow.FindName("SearchButton") :?> Button
        let clearButton = parsedWindow.FindName("ClearButton") :?> Button
        let saveButton = parsedWindow.FindName("SaveButton") :?> Button
        let loadButton = parsedWindow.FindName("LoadButton") :?> Button
        
        addButton.Click.Add(fun _ -> this.OnAdd())
        updateButton.Click.Add(fun _ -> this.OnUpdate())
        deleteButton.Click.Add(fun _ -> this.OnDelete())
        searchButton.Click.Add(fun _ -> this.OnSearch())
        clearButton.Click.Add(fun _ -> this.OnClear())
        saveButton.Click.Add(fun _ -> this.OnSave())
        loadButton.Click.Add(fun _ -> this.OnLoad())
        
        // Handle list selection
        resultsListBox.SelectionChanged.Add(fun _ -> this.OnSelectionChanged())
        
        // Load initial dictionary if exists
        this.TryLoadInitialDictionary()
        this.RefreshList()
    
    /// Gets the current translation direction
    member private this.GetDirection() =
        match directionComboBox.SelectedIndex with
        | 0 -> ArToEn
        | _ -> EnToAr
    
    /// Updates the status message
    member private this.SetStatus(message: string, isError: bool) =
        statusText.Text <- message
        statusText.Foreground <- 
            if isError then 
                Media.SolidColorBrush(Media.Color.FromRgb(243uy, 139uy, 168uy)) // Red
            else 
                Media.SolidColorBrush(Media.Color.FromRgb(166uy, 227uy, 161uy)) // Green
    
    /// Refreshes the results list
    member private this.RefreshList() =
        let direction = this.GetDirection()
        let entries = Crud.getAllEntries direction currentDictionary
        resultsListBox.Items.Clear()
        for (word, meaning) in entries do
            resultsListBox.Items.Add(sprintf "%s → %s" word meaning) |> ignore
    
    /// Tries to load dictionary from default file
    member private this.TryLoadInitialDictionary() =
        let defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dictionary.json")
        match FileIO.loadFromJson defaultPath with
        | Ok dict -> 
            currentDictionary <- dict
            this.SetStatus("Dictionary loaded successfully.", false)
        | Error _ -> 
            this.SetStatus("Ready. No dictionary file found.", false)
    
    /// Handles selection change in results list
    member private this.OnSelectionChanged() =
        match resultsListBox.SelectedItem with
        | null -> ()
        | item ->
            let text = item.ToString()
            let parts = text.Split([| " → " |], StringSplitOptions.None)
            if parts.Length >= 2 then
                wordTextBox.Text <- parts.[0]
                meaningTextBox.Text <- parts.[1]
    
    /// Add button handler
    member private this.OnAdd() =
        let word = wordTextBox.Text
        let meaning = meaningTextBox.Text
        let direction = this.GetDirection()
        
        match Crud.addWord word meaning direction currentDictionary with
        | Ok newDict ->
            currentDictionary <- newDict
            this.RefreshList()
            this.SetStatus(sprintf "Added '%s' successfully." word, false)
            wordTextBox.Clear()
            meaningTextBox.Clear()
        | Error err ->
            this.SetStatus(Error.toMessage err, true)
    
    /// Update button handler
    member private this.OnUpdate() =
        let word = wordTextBox.Text
        let meaning = meaningTextBox.Text
        let direction = this.GetDirection()
        
        match Crud.updateWord word meaning direction currentDictionary with
        | Ok newDict ->
            currentDictionary <- newDict
            this.RefreshList()
            this.SetStatus(sprintf "Updated '%s' successfully." word, false)
        | Error err ->
            this.SetStatus(Error.toMessage err, true)
    
    /// Delete button handler
    member private this.OnDelete() =
        let word = wordTextBox.Text
        let direction = this.GetDirection()
        
        match Crud.deleteWord word direction currentDictionary with
        | Ok newDict ->
            currentDictionary <- newDict
            this.RefreshList()
            this.SetStatus(sprintf "Deleted '%s' successfully." word, false)
            wordTextBox.Clear()
            meaningTextBox.Clear()
        | Error err ->
            this.SetStatus(Error.toMessage err, true)
    
    /// Search button handler
    member private this.OnSearch() =
        let query = wordTextBox.Text
        let direction = this.GetDirection()
        let results = Search.searchAll query direction currentDictionary
        
        resultsListBox.Items.Clear()
        for (word, meaning) in results do
            resultsListBox.Items.Add(sprintf "%s → %s" word meaning) |> ignore
        
        this.SetStatus(sprintf "Found %d results." (List.length results), false)
    
    /// Clear button handler
    member private this.OnClear() =
        wordTextBox.Clear()
        meaningTextBox.Clear()
        this.RefreshList()
        this.SetStatus("Cleared inputs.", false)
    
    /// Save button handler
    member private this.OnSave() =
        let dialog = Microsoft.Win32.SaveFileDialog()
        dialog.Filter <- "JSON files (*.json)|*.json"
        dialog.DefaultExt <- ".json"
        dialog.FileName <- "dictionary.json"
        
        if dialog.ShowDialog().GetValueOrDefault() then
            match FileIO.saveToJson dialog.FileName currentDictionary with
            | Ok () ->
                this.SetStatus(sprintf "Saved to %s" dialog.FileName, false)
            | Error err ->
                this.SetStatus(Error.toMessage err, true)
    
    /// Load button handler
    member private this.OnLoad() =
        let dialog = Microsoft.Win32.OpenFileDialog()
        dialog.Filter <- "JSON files (*.json)|*.json"
        dialog.DefaultExt <- ".json"
        
        if dialog.ShowDialog().GetValueOrDefault() then
            match FileIO.loadFromJson dialog.FileName with
            | Ok dict ->
                currentDictionary <- dict
                this.RefreshList()
                this.SetStatus(sprintf "Loaded from %s" dialog.FileName, false)
            | Error err ->
                this.SetStatus(Error.toMessage err, true)
