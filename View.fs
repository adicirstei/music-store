module SuaveMusicStore.View

open Suave.Html


let cssLink href = linkAttr [ "href", href; " rel", "stylesheet"; " type", "text/css" ]

let divId id = divAttr ["id", id]
let h1 xml = tag "h1" [] xml
let aHref href = tag "a" ["href", href]

let home = [
  text "<h1>Home</h1>"
]


let index container =
  html [
    head [
      title "Suave Music Store"
      cssLink "/Site.css"
    ]
    body [
      divId "header" [
        h1 (aHref Path.home (text "F# Suave Music Store"))
      ]
      divId "main" container

      divId "footer" [
        text "built with"
        aHref "http://fsharp.org" (text "F#")
        text " and "
        aHref "http://suave.io" (text "Suave.IO")
      ]
    ]
  ]
  |> xmlToString
