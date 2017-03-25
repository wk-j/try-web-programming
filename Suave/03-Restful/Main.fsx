#r "../../packages/Suave/lib/net40/Suave.dll"

open Suave
open Suave.Json
open System.Runtime.Serialization


type Foo = 
    { 
        //[<field: DataMember(Name="Foot")>]
        Foo : string }

type Bar = 
    { Bar : string }


let web = mapJson (fun (a: Foo) -> { Bar = a.Foo })

startWebServer defaultConfig web 


// curl -X POST -d '{"foo":"xyz"}' http://localhost:8080/ -w "\n"


