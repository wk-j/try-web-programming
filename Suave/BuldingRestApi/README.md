
## Main.fsx

```fsharp
#r "../../packages/Suave/lib/net40/Suave.dll"
#r "../../packages/Newtonsoft.Json/lib/net40/Newtonsoft.Json.dll"

open Suave

open Suave.Successful
open Suave.Writers
open Suave.Operators
open Suave.Successful
open Suave.Filters

open System.Collections.Generic

open Newtonsoft.Json
open Newtonsoft.Json.Serialization

[<AutoOpen>]
module Db = 
    type Person = {
        Id: int
        Name: string
        Age: int
        Email: string
    }

    let private peopleStorage = Dictionary<string, Person>()
    let getPeople() = peopleStorage.Values  |> Seq.map id

[<AutoOpen>]
module Restful =
    type 'a RestResource = {
        GetAll : unit -> 'a seq
    }

    let JSON v =
        let jsonSerializeSettings = JsonSerializerSettings()
        jsonSerializeSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializeSettings) 
        |> OK >=> setMimeType "application/json; charset=utf-8"

    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let getAll = resource.GetAll() |> JSON
        path resourcePath >=> GET >=> getAll


let personWebPart = rest "people" {
    GetAll = Db.getPeople
}

startWebServer defaultConfig personWebPart 
```
