module Benchmarks.WebSharper.SqlProvider.Fortunes.Data

open Benchmarks.WebSharper.SqlProvider.Data
open Benchmarks.WebSharper.SqlProvider.Fortunes.Types

let private queryAllFortunes (dataContext: Db.dataContext) =
    dataContext.``[PUBLIC].[FORTUNE]`` |> Seq.map (fun row -> { id = row.id; message = row.message })

let allFortunes: seq<Fortune> = dataContext () |> queryAllFortunes



