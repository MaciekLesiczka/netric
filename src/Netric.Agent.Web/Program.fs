// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open Netric.Agent.Web.App
[<EntryPoint>]
let main argv = 
    let cancel = start()

    System.Console.ReadLine() |> ignore

    printfn "%A" argv
    0 // return an integer exit code
