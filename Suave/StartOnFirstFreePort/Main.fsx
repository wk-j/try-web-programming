#r "../../packages/Suave/lib/net40/Suave.dll"

// http://fssnip.net/7Po/title/Start-Suave-server-on-first-free-port

// Not working

open Suave
open System

let app = Successful.OK "Hello world!"

let log x = 
    let str = sprintf "%A" x
    Console.WriteLine(str)

let startServer() =
    Async.FromContinuations(fun (cont, _, _) ->

        log "From continuations ..."

        let startedEvent = Event<_>()
        startedEvent.Publish.Add(cont)

        async {
            let rnd = System.Random()
            while true do 
                let port = 8000 + rnd.Next(2000)
                let local = Suave.Http.HttpBinding.createSimple HTTP "127.0.0.1" port

                let config = { defaultConfig with bindings = [local];}
                let started, start = startWebServerAsync config app
                async {
                    let! running = started
                    startedEvent.Trigger(running) 
                } |> Async.Start

                try
                    log port
                    do! start
                with :? System.Net.Sockets.SocketException as ex -> 
                    log ex.Message
        } |> Async.Start
    )

log "Start server ..."
startServer() |> Async.RunSynchronously

Console.ReadLine()
