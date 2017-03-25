#r "../../packages/Suave/lib/net40/Suave.dll"

open Suave
open Suave.Successful

let sleep milliseconcds message: WebPart = 
    fun (x : HttpContext) ->
        async {
            do! Async.Sleep milliseconcds
            return! OK message x
        }