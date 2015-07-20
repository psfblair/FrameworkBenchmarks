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
    | [<EndPoint "GET /queries">]   MultipleQuery of int
    | [<EndPoint "GET /updates">]   DataUpdate of int
//    | [<EndPoint "GET /queries">][<Query("queries")>]  MultipleQuery of int
//    | [<EndPoint "GET /updates">][<Query("queries")>]  DataUpdate of int


let BenchmarksApplication =
    let random = System.Random()

    Warp.CreateApplication (fun ctx endpoint ->
        match endpoint with
        | Endpoints.Plaintext   -> Hello.plaintextContent |> CustomContentAsync 
        | Endpoints.Json        -> Hello.jsonContent      |> JsonContentAsync 
        | Endpoints.Fortunes    -> Fortune.fortuneContent |> PageContentAsync 

        | Endpoints.SingleQuery -> World.singleQueryContent random |> JsonContentAsync

        | Endpoints.MultipleQuery (numberOfQueries)  -> World.multipleQueryContent random numberOfQueries  |> JsonContentAsync
        | Endpoints.DataUpdate    (numberOfQueries)  -> World.multipleUpdateContent random numberOfQueries |> JsonContentAsync
    )


[<EntryPoint>]
do Warp.RunAndWaitForInput(BenchmarksApplication) |> ignore