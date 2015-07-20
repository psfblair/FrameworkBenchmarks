module Benchmarks.WebSharper.SqlProvider.Tests.Data

open Benchmarks.WebSharper.SqlProvider.Worlds.Data
open Benchmarks.WebSharper.SqlProvider.Data
open Swensen.Unquote
open NUnit.Framework

[<Test>]
let ``should retrieve a record from a single random query`` () = 
    // Will fail if no results are returned, with a System.ArgumentException.
    worldForRandomId (System.Random()) |> ignore

[<Test>]
let ``should retrieve multiple records from a multi-random-query`` () = 
    let numberOfQueries = 500
    let records = numberOfQueries |> multipleRandomWorlds (System.Random()) |> Async.RunSynchronously
    test <@ Array.length records = numberOfQueries @>

[<Test>]
let ``should update one row in the database`` () =
    let id = 1
    let updatedValue = -1

    let record = worldWithId id
    test <@ record.id = id @>
    let oldValue = record.randomNumber

    let updatedRecord = updatedValue |> updateWorldWithId record.id 
    test <@ updatedRecord.id = record.id @>
    test <@ updatedRecord.randomNumber = updatedValue @>

    let confirm = worldWithId id
    test <@ confirm.id = updatedRecord.id @>
    test <@ confirm.randomNumber = updatedRecord.randomNumber @>

    oldValue |> updateWorldWithId record.id  |> ignore //Just to reset the db

    let reverted = worldWithId id // Make sure we reset.
    test <@ reverted.randomNumber = oldValue @>

[<Test>]
let ``should update multiple rows in the database`` () =
    let numberOfQueries = 100
    let dbContext = dataContext()
    let oldData = query { for row in dbContext.``[PUBLIC].[WORLD]`` do select (row.ID, row.RANDOMNUMBER) } 
                    |> Seq.cast |> Map.ofSeq
    let newData = numberOfQueries |> updateMultipleRandomWorldsWithRandomValues (System.Random()) |> Async.RunSynchronously

    // This will fail if one of our random numbers is the same as the old value. 1/100 chance for any given test run.
    newData |> Array.iter (fun newRow -> test <@ Map.find newRow.id oldData <> newRow.randomNumber @>) 