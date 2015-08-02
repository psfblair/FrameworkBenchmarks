module Benchmarks.WebSharper.SqlProvider.Router

open Benchmarks.WebSharper.SqlProvider
open Benchmarks.WebSharper.SqlProvider.Fortunes.View
open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Content
open WebSharper.Sitelets.ActionEncoding
open System

type Endpoints =
    | [<EndPoint "GET /plaintext">] Plaintext
    | [<EndPoint "GET /json">]      Json
    | [<EndPoint "GET /fortunes">]  Fortunes
    | [<EndPoint "GET /db">]        SingleQuery
    | [<EndPoint "GET /queries">][<Query("queries")>]   MultipleQuery of queries: int
    | [<EndPoint "GET /updates">][<Query("updates")>]   DataUpdate of updates: int

let toPage (pageContent: ContentForTemplate) = Content.Page(Doctype = pageContent.Doctype, 
                                                            Title = pageContent.Title, 
                                                            Body = pageContent.Body)

let (>>=) (wrapped: Async<'a>) (wrapper: 'a -> Async<'b>) =
    async { let! contents = wrapped 
            return! wrapper contents }

[<Website>]
let BenchmarksApplication =
    let random = System.Random()

    Sitelet.InferWithCustomErrors <| fun ctx -> function
        | Success Endpoints.Plaintext   -> Hello.plaintextContent |> Content.Text 
        | Success Endpoints.Json        -> Hello.jsonContent |> Content.Json
        | Success Endpoints.Fortunes    -> Fortune.fortuneContent |> toPage

        | Success Endpoints.SingleQuery -> World.singleQueryContent random |> Content.Json

        | Success(Endpoints.MultipleQuery numberOfQueries)
                                        -> World.multipleQueryContent random numberOfQueries >>= Content.Json

        | MissingQueryParameter((Endpoints.MultipleQuery numberOfQueries), "queries")
                                        -> World.multipleQueryContent random 1 >>= Content.Json

        | Success(Endpoints.DataUpdate numberOfUpdates)
                                        -> World.multipleUpdateContent random numberOfUpdates >>= Content.Json

        | MissingQueryParameter((Endpoints.DataUpdate numberOfUpdates), "updates") 
                                        -> World.multipleUpdateContent random 1 >>= Content.Json
        | _                             -> Content.NotFound

#if WARP
[<EntryPoint>]
do Warp.RunAndWaitForInput (BenchmarksApplication, false, "http://127.0.0.1:9000") |> ignore
#else
#endif