module SuaveMusicStore.App
//#r "packages/Suave/lib/net40/Suave.dll"
//#r "packages/Suave.Experimental/lib/net40/Suave.Experimental.dll"


open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config

open Suave.Filters
open Suave.Operators
open Suave.RequestErrors


let browse =
  request (fun r ->
    match r.queryParam "genre" with
    | Choice1Of2 genre -> OK (sprintf "Genre: %s" genre)
    | Choice2Of2 msg -> BAD_REQUEST msg
  )


let webPart =
  choose [
    path Path.home >=> (OK View.index)
    pathRegex "(.*)\.(css|png)" >=> Files.browseHome
    path Path.Store.overview >=> (OK "Store")
    path Path.Store.browse >=> browse
    pathScan Path.Store.details (fun id -> OK (sprintf "Details %d" id))
  ]


startWebServer defaultConfig webPart
