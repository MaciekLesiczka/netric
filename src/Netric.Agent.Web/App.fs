namespace Netric.Agent.Web
    open Suave
    open Suave.Files
    open Suave.Filters
    open Suave.Operators        
    open System
    open System.Threading    
    open Settings
    open Suave.Api

    module App =
                        
        let pages = ["/"
                     "/home"
                     "/settings"] |> List.map (fun x-> path x >=> file "index.html")                        
        let app =
          choose [ GET >=> choose pages
                   path "/api/settings" >=> choose[ GET >=> query Settings.get
                                                    POST >=> command Settings.set ]
                   GET >=> Files.browseHome
                   RequestErrors.NOT_FOUND "Page not found." ]
            
        let start () = 
            let cts = new CancellationTokenSource()
            let token = cts.Token
            let config = { defaultConfig with cancellationToken = token }
            startWebServerAsync defaultConfig app
            |> snd
            |> Async.StartAsTask 
            |> ignore
            cts
    