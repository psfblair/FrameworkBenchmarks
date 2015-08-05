module Benchmarks.WebSharper.SqlProvider.Routes

open WebSharper.Sitelets

type Endpoints =
    | [<EndPoint "GET /plaintext">] Plaintext
    | [<EndPoint "GET /json">]      Json
    | [<EndPoint "GET /fortunes">]  Fortunes
    | [<EndPoint "GET /db">]        SingleQuery
    | [<EndPoint "GET /queries">][<Query("queries")>]   MultipleQuery of queries: int
    | [<EndPoint "GET /updates">][<Query("updates")>]   DataUpdate of updates: int

