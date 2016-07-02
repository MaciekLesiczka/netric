namespace Netric.Agent.Web
    open Suave
    open Suave.Files
    open Suave.Filters
    open Suave.Operators
    open Suave.DotLiquid
    
    open Suave.Model.Binding

    open DotLiquid
    open System
    open System.Threading
    
    open Settings

    module App =
        
        type SettingsModel = {settings:Attributes;completed:bool }
                   
        let settings = 
          choose[ GET >=> warbler (fun ctx ->        
                       let settings = Settings.get()                       
                       page "index.html" {settings=settings;completed=false})
                  POST >=> request (fun req ->                              
                       let result,_ = Settings.set req.form                       
                       page "index.html" {settings=result;completed=true})
                ]
        
        let app =
          choose [ path "/" >=> settings
                   GET >=> choose [ Files.browseHome ]              
                   RequestErrors.NOT_FOUND "Page not found." ]
            
        let start () = 
            let cts = new CancellationTokenSource()
            let token = cts.Token
            let config = { defaultConfig with cancellationToken = token}
            startWebServerAsync defaultConfig app
            |> snd
            |> Async.StartAsTask 
            |> ignore
            cts
    