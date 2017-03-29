
## Continuation.fsx

```fsharp
open System

// Primitive that delays the workflow
let sleep(time) = 
  // 'FromContinuations' is the basic primitive for creating workflows
  Async.FromContinuations(fun (cont, econt, ccont) ->
    Console.WriteLine("Sleep")
    // This code is called when workflow (this operation) is executed
    let tmr = new System.Timers.Timer(time, AutoReset=true)
    tmr.Elapsed.Add(fun _ -> 
        // Run the rest of the computation
        cont())
    tmr.Start() )

let sleep2(time) = 
  // 'FromContinuations' is the basic primitive for creating workflows
  Async.FromContinuations(fun (cont, econt, ccont) ->
    Console.WriteLine("Sleep2")
    // This code is called when workflow (this operation) is executed
    let tmr = new System.Timers.Timer(time, AutoReset=true)
    tmr.Elapsed.Add(fun _ -> 
        // Run the rest of the computation
        cont())
    tmr.Start() )

sleep(100.0) |> Async.RunSynchronously
sleep2(100.0) |> Async.RunSynchronously


#r "../../packages/Suave/lib/net40/Suave.dll"
open Suave


let conf = defaultConfig
// startWebServer conf (Successful.OK "Hello, world!")
```
