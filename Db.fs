module SuaveMusicStore.Db

open FSharp.Data.Sql

type Sql =
  SqlDataProvider<
    ConnectionString = "Server=ROBUCNTSQL01;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true",
    DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER >


let getContext() = Sql.GetDataContext()

type DbContext = Sql.dataContext
type Album = DbContext.``dbo.AlbumsEntity``
type Genre = DbContext.``dbo.GenresEntity``
type AlbumDetails = DbContext.``dbo.AlbumDetailsEntity``
type Artist = DbContext.``dbo.ArtistsEntity``

let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getArtists (ctx : DbContext) : Artist list =
  ctx.Dbo.Artists |> Seq.toList

let getGenres (ctx : DbContext) : Genre list =
  ctx.Dbo.Genres |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list =
  query {
    for album in ctx.Dbo.Albums do
      join genre in ctx.Dbo.Genres on (album.GenreId = genre.GenreId)
      where (genre.Name = genreName)
      select album
  }
  |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option =
  query {
    for album in ctx.Dbo.AlbumDetails do
      where (album.AlbumId = id)
      select album
  } |> firstOrNone

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list =
  ctx.Dbo.AlbumDetails |> Seq.toList


let getAlbum id (ctx:DbContext) : Album option =
  query {
    for album in ctx.Dbo.Albums do
      where (album.AlbumId = id)
      select album
  } |> firstOrNone

let deleteAlbum (album: Album) (ctx :DbContext) =
  album.Delete()
  ctx.SubmitUpdates()

let createAlbum (artistId, genreId, price, title) (ctx : DbContext) =
  ctx.Dbo.Albums.Create (artistId, genreId, price, title) |> ignore
  ctx.SubmitUpdates()
