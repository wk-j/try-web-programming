#r "../../packages/Suave/lib/net40/Suave.dll"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open System.Threading
open System


let app =
  choose
    [ GET >=> choose
        [ path "/hello" >=> OK "Hello GET"
          path "/goodbye" >=> OK "Good bye GET" ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] ]

let cts = new CancellationTokenSource()
let conf = { defaultConfig with cancellationToken = cts.Token }
let listening, server = startWebServerAsync conf app 
Async.Start(server, cts.Token)
printfn "Make requests now"
Console.ReadKey true |> ignore
cts.Cancel()

