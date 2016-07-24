open Netric.Agent.Api.App
[<EntryPoint>]
let main argv = 
    let cancel = start()

    System.Console.ReadLine() |> ignore

    printfn "%A" argv
    0 // return an integer exit code
