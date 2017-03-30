
#r "./node_modules/fable-core/Fable.Core.dll"
#r "./node_modules/fable-elmish/Fable.Elmish.dll"
#r "./node_modules/fable-elmish-react/Fable.Elmish.React.dll"
#r "./node_modules/fable-react/Fable.React.dll"

//module App

(**
 - title: Counter
 - tagline: The famous Increment/Decrement ported from Elm
*)

//open Fable.Core
//open Fable.Import
open Elmish

// MODEL
type Msg =
  | Increment
  | Decrement

let init () = 0

// UPDATE
let update (msg:Msg) count =
  match msg with
  | Increment -> count + 1
  | Decrement -> count - 1

// rendering views with React
//open Fable.Core.JsInterop
open Fable.Helpers.React.Props
module R = Fable.Helpers.React

let view count dispatch =
  let onClick msg =
    OnClick <| fun _ -> msg |> dispatch

  R.div []
    [ R.button [ onClick Decrement ] [ R.str "-" ]
      R.div [] [ R.str (string count) ]
      R.button [ onClick Increment ] [ R.str "+" ] ]

open Elmish.React

// App
Program.mkSimple init update view
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.run