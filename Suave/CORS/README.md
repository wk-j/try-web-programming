
## Main.fsx

```fsharp
#r "../../packages/Suave/lib/net40/Suave.dll"

open Suave
open System
open Suave.Successful
open Suave.Filters
open Suave.Operators
open Suave.Writers

let headers =
    setHeader "Access-Control-Allow-Origin" "*"
    >=> setHeader "Access-Control-Allow-Headers" "Content-Type"

let allowCors(): WebPart = 
    choose 
        [ OPTIONS >=> fun context -> context |> (headers >=> OK "Approved") ]

let website: WebPart =
    let ok = GET >=> OK "Hello"
    choose [ allowCors(); ok ] 

startWebServer defaultConfig website

```
