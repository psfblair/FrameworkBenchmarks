module Benchmarks.WebSharper.SqlProvider.World

open Benchmarks.WebSharper.SqlProvider.Worlds.Data
open WebSharper.Sitelets

let private capAt500 numQueries = if numQueries < 1 then 1 else if numQueries > 500 then 500 else numQueries

let singleQueryContent random  =  worldForRandomId random 

let multipleQueryContent random numberOfQueries  = 
    numberOfQueries |> capAt500 |> multipleRandomWorlds random

let multipleUpdateContent random numberOfQueries = 
    numberOfQueries |> capAt500 |> updateMultipleRandomWorldsWithRandomValues random 



