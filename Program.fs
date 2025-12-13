/// Program entry point - Launches the WPF application
namespace DictionaryApp

open System
open System.Windows

module Program =
    
    [<STAThread>]
    [<EntryPoint>]
    let main argv =
        let app = Application()
        let window = MainWindow()
        app.Run(window)
