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
  >=> Writers.setMimeType "text/html; charset=utf-8"

let browse =
  request (fun r ->
    match r.queryParam Path.Store.browseKey with
    | Choice1Of2 genre ->
      Db.getContext()
      |> Db.getAlbumsForGenre genre
      |> View.browse genre
      |> html
    | Choice2Of2 msg -> BAD_REQUEST msg
  )



let overview = warbler (fun _ ->
  Db.getContext()
  |> Db.getGenres
  |> List.map (fun g -> g.Name)
  |> View.store
  |> html
)


let details id =
  match Db.getAlbumDetails id (Db.getContext()) with
  | Some album ->
    html (View.details album)
  | None ->
    never


let manage = warbler (fun _ ->
  Db.getContext()
  |> Db.getAlbumsDetails
  |> View.manage
  |> html)


let webPart =
  choose [
    path Path.home >=> html View.home
    path Path.Store.overview >=> overview
    path Path.Store.browse >=> browse
    pathScan Path.Store.details details
    pathScan Path.Admin.deleteAlbum deleteAlbum

    path Path.Admin.manage >=> manage

    pathRegex "(.*)\.(css|png|gif)" >=> Files.browseHome
    html View.notFound
  ]


let deleteAlbum id =
  let ctx = Db.getContext()
  match Db.getAlbum id ctx with
    | Some album ->
      choose [
        GET >=> warbler (fun _ ->
          html (View.deleteAlbum album.Title))
        POST >=> warbler (fun _ ->
          Db.deleteAlbum album ctx;
          Redirection.FOUND Path.Admin.manage)
      ]

    | None ->
      never





startWebServer defaultConfig webPart
