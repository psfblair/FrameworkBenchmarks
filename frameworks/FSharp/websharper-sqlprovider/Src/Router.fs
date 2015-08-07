module Benchmarks.WebSharper.SqlProvider.Router

open Benchmarks.WebSharper.SqlProvider
open Benchmarks.WebSharper.SqlProvider.Routes
open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Content
open WebSharper.Sitelets.ActionEncoding
open System
open System.Net
open System.Net.NetworkInformation

let (>>=) (wrapped: Async<'a>) (wrapper: 'a -> Async<'b>) =
    async { let! contents = wrapped 
            return! wrapper contents }

[<Website>]
let BenchmarksApplication =
    let random = System.Random()

    Sitelet.InferWithCustomErrors <| fun ctx -> function
        | Success Endpoints.Plaintext   -> Hello.plaintextContent |> Content.Text 
        | Success Endpoints.Json        -> Hello.jsonContent |> Content.Json
        | Success Endpoints.Fortunes    -> Fortune.fortuneContent 

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
let hostName = Dns.GetHostName()
let domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName

let fullyQualifiedName = 
        if (hostName.EndsWith(domainName) || domainName = "(none)") 
        then hostName
        else domainName + "." + hostName

let ipAddresses = Dns.GetHostAddresses(fullyQualifiedName) 
                    |> Array.map (fun x -> x.ToString()) 
                    |> Set.ofArray

let allAddresses = ipAddresses |> Set.add fullyQualifiedName |> Set.add "127.0.0.1" |> Set.add "localhost"

let urlsToListenOn = allAddresses |> Set.map (fun address -> "http://" + address + ":8085/") |> Set.toList

[<EntryPoint>]
do Warp.RunAndWaitForInput (BenchmarksApplication, urls = urlsToListenOn) |> ignore
#else
#endif