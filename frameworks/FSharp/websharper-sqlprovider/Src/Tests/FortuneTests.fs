module Benchmarks.WebSharper.SqlProvider.Tests.Controller

open Benchmarks.WebSharper.SqlProvider.Fortune
open Benchmarks.WebSharper.SqlProvider.Fortunes.Data
open Benchmarks.WebSharper.SqlProvider.Fortunes.Types
open Swensen.Unquote
open NUnit.Framework

[<Test>]
let ``should retrieve all the fortunes in the database`` () = 
    let records = allFortunes |> Seq.sortBy (fun record -> record.id)
    test <@ Seq.length records = 12 @>
    test <@ (Seq.head records).message = "fortune: No such file or directory" @>

[<Test>]
let ``should sort fortunes`` () = 
    let testSort (fortune: Fortune, nextFortune: Fortune) = test <@ fortune.message < nextFortune.message @>
    fortunes |> Seq.pairwise |> Seq.iter testSort 
