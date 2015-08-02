module Benchmarks.WebSharper.SqlProvider.Router

open Benchmarks.WebSharper.SqlProvider
open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Content
open System

type Endpoints =
    | [<EndPoint "GET /plaintext">] Plaintext
    | [<EndPoint "GET /json">]      Json
    | [<EndPoint "GET /fortunes">]  Fortunes
    | [<EndPoint "GET /db">]        SingleQuery
    | [<EndPoint "GET /queries">]   MultipleQuery of numberOfQueries: int
    | [<EndPoint "GET /queries">]   MultipleQueryNoParam
    | [<EndPoint "GET /queries">]   MultipleQueryBadParam of badParameter: string
    | [<EndPoint "GET /updates">]   DataUpdate of numberOfQueries: int
    | [<EndPoint "GET /updates">]   DataUpdateNoParam
    | [<EndPoint "GET /updates">]   DataUpdateBadParam of badParameter: string

[<Website>]
let BenchmarksApplication =
    let random = System.Random()

    Application.MultiPage <| fun ctx -> function
        | Endpoints.Plaintext   -> Hello.plaintextContent |> Content.Text 
        | Endpoints.Json        -> Hello.jsonContent |> Content.Json
        | Endpoints.Fortunes    -> let pageContent = Fortune.fortuneContent
                                   Content.Page(Doctype = pageContent.Doctype, Title = pageContent.Title, Body = pageContent.Body)

        | Endpoints.SingleQuery                     -> World.singleQueryContent random |> Content.Json

        | Endpoints.MultipleQuery (numberOfQueries) -> World.multipleQueryContent random numberOfQueries |> Content.Json

        | Endpoints.MultipleQueryNoParam 
        | Endpoints.MultipleQueryBadParam (_)       -> World.multipleQueryContent random 1 |> Content.Json

        | Endpoints.DataUpdate    (numberOfQueries) -> World.multipleUpdateContent random numberOfQueries |> Content.Json

        | Endpoints.DataUpdateNoParam 
        | Endpoints.DataUpdateBadParam (_)          -> World.multipleUpdateContent random 1 |> Content.Json

#if WARP
[<EntryPoint>]
do Warp.RunAndWaitForInput (BenchmarksApplication, false, "http://127.0.0.1:9000") |> ignore
#else
#endif