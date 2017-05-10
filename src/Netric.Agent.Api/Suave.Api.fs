namespace Suave
    open Suave.Operators       
    open Newtonsoft.Json
    
    module Api = 
        let toJson o = o |> JsonConvert.SerializeObject 
                         |> Successful.OK 
                         >=> Writers.setMimeType "application/json"
                
        let query (f:Unit->'o)= warbler (fun _ -> f() |> toJson)
               
        let command<'i,'o> (f:'i->'o) = 
            request (fun r -> match r.form with                                                               
                                    | (x,_)::_ -> JsonConvert.DeserializeObject<'i>(x) |> f |> toJson
                                    | _ -> RequestErrors.BAD_REQUEST "invalid input")