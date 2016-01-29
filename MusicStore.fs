module SuaveMusicStore.App
//#r "packages/Suave/lib/net40/Suave.dll"
//#r "packages/Suave.Experimental/lib/net40/Suave.Experimental.dll"

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

let html container =
  OK (View.index container)

let browse =
  request (fun r ->
    match r.queryParam Path.Store.browseKey with
    | Choice1Of2 genre -> html (View.browse genre)
    | Choice2Of2 msg -> BAD_REQUEST msg
  )



let overview =
  Db.getContext()
  |> Db.getGenres
  |> List.map (fun g -> g.Name)
  |> View.store
  |> html

let webPart =
  choose [
    path Path.home >=> html View.home
    path Path.Store.overview >=> overview
    path Path.Store.browse >=> browse
    pathScan Path.Store.details (fun id -> html (View.details id))

    pathRegex "(.*)\.(css|png)" >=> Files.browseHome
  ]

startWebServer defaultConfig webPart
