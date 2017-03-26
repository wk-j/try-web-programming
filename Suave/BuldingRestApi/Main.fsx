#r "../../packages/Suave/lib/net40/Suave.dll"
#r "../../packages/Newtonsoft.Json/lib/net40/Newtonsoft.Json.dll"

// http://blog.tamizhvendan.in/blog/2015/06/11/building-rest-api-in-fsharp-using-suave/

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

    let private storage = Dictionary<int, Person>()
    let getPeople() = storage.Values  |> Seq.map (fun x -> x)

    let createPerson person =
        let id = storage.Values.Count + 1
        let newPerson = {
            Id = id
            Name = person.Name
            Age = person.Age
            Email = person.Email
        }

        storage.Add(id, newPerson)
        newPerson

    let updatePersonById id person =
        if storage.ContainsKey id then
            let update = {
                Id = id
                Name = person.Name
                Age = person.Age
                Email = person.Email
            }

            storage.[id] <- update
            Some update
        else None

    let updatePerson person =
        updatePersonById person.Id person

    let deletePerson id =
        storage.Remove(id) |> ignore

[<AutoOpen>]
module Restful =
    type 'a RestResource = {
        GetAll : unit -> 'a seq
        Create: 'a -> 'a
        Update: 'a -> 'a option
        Delete: int -> unit
    }

    let JSON v =
        let jsonSerializeSettings = JsonSerializerSettings()
        jsonSerializeSettings.ContractResolver <- CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializeSettings) 
        |> OK >=> setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a>(req: HttpRequest) =
        let getString rawForm = 
            System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    let rest resourceName resource =

        let resourcePath = "/" + resourceName
        let badRequest = RequestErrors.BAD_REQUEST "Resource noto found"
        let getAll = resource.GetAll() |> JSON

        let resourceIdPath = 
            PrintfFormat<(int -> string), unit, string, string, int> (resourcePath + "/%d")

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        path resourcePath >=>
            choose [
                GET >=> getAll
                POST >=> 
                    request (getResourceFromReq >> resource.Create >> JSON)
                PUT >=>
                    request (getResourceFromReq >> resource.Update >> handleResource badRequest)
                DELETE >=>
                    pathScan resourceIdPath deleteResourceById
            ]

let a = GET >=> OK "xxx"


let personWebPart = rest "people" {
    GetAll = Db.getPeople
    Create = Db.createPerson
    Update = Db.updatePerson
    Delete = Db.deletePerson
}

startWebServer defaultConfig personWebPart 