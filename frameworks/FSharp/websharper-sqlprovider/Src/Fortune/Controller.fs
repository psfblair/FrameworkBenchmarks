module Benchmarks.WebSharper.SqlProvider.Fortune

open Benchmarks.WebSharper.SqlProvider.Fortunes.Types
open Benchmarks.WebSharper.SqlProvider.Fortunes.Data
open Benchmarks.WebSharper.SqlProvider.Fortunes.View

open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Content
open System

let private additionalFortune = seq { yield { id = 0; message = "Additional fortune added at request time." } }
let private sortedByMessage = Seq.sortBy (fun fortune -> fortune.message)

//Would like this to be private but want to test it
let internal fortunes = additionalFortune 
                        |> Seq.append allFortunes
                        |> sortedByMessage

let fortuneContent context = async { return fortunes |> toFortunePageContent }
