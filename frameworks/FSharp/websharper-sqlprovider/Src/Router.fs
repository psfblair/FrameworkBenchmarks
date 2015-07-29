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


type BenchmarksApplication() =
    interface IWebsite<Endpoints> with
        member this.Sitelet = 
            let random = System.Random()

            Sitelet.Infer <| function
                | Endpoints.Plaintext   -> Hello.plaintextContent |> CustomContentAsync 
                | Endpoints.Json        -> Hello.jsonContent      |> JsonContentAsync 
                | Endpoints.Fortunes    -> Fortune.fortuneContent |> Content.WithTemplateAsync Fortunes.View.fortuneTemplate 

                | Endpoints.SingleQuery -> World.singleQueryContent random |> JsonContentAsync

                | Endpoints.MultipleQuery (numberOfQueries)
                                        -> World.multipleQueryContent random numberOfQueries  |> JsonContentAsync
                | Endpoints.MultipleQueryNoParam | Endpoints.MultipleQueryBadParam (_) 
                                        -> World.multipleQueryContent random 1  |> JsonContentAsync
                | Endpoints.DataUpdate    (numberOfQueries)  
                                        -> World.multipleUpdateContent random numberOfQueries |> JsonContentAsync
                | Endpoints.DataUpdateNoParam | Endpoints.DataUpdateBadParam (_)
                                        -> World.multipleUpdateContent random 1 |> JsonContentAsync

        member this.Actions = []

[<assembly: WebsiteAttribute(typeof<BenchmarksApplication>)>] 
do ()
