module Benchmarks.WebSharper.SqlProvider.Worlds.Data

open Benchmarks.WebSharper.SqlProvider.Data
open Benchmarks.WebSharper.SqlProvider.Worlds.Types
open System

(* ********************************************* PRIVATE ************************************************ *)
let private next (random: Random) = random.Next(1, 10000)

let private singleQueryFromWorld (dbContext: Db.dataContext) id = 
    query {
       for row in dbContext.``[PUBLIC].[WORLD]`` do  
       where (row.ID = id)
       select { id = row.ID; randomNumber = row.RANDOMNUMBER }
    } |> Seq.head


let private singleRandomQueryFromWorld (dbContext: Db.dataContext) (random: Random) =
    next random |> singleQueryFromWorld dbContext


let private updateSingleWorldRecord (dbContext: Db.dataContext) id update = 
    let record = query {
                   for row in dbContext.``[PUBLIC].[WORLD]`` do  
                   where (row.ID = id)
                 } |> Seq.head

    let oldRandom = record.RANDOMNUMBER    // Make sure record is actually fetched
    if update <> oldRandom then            // SQLProvider doesn't like it if we update unchanged rows.
        record.RANDOMNUMBER <- update
        dbContext.SubmitUpdates()
    { id = id; randomNumber = update }


let private updateRandomWorldRecordWithRandomValue (dbContext: Db.dataContext) (random: Random) = 
    let key: int = next random
    next random |> updateSingleWorldRecord dbContext key 


(* ********************************************* PUBLIC ************************************************ *)
let worldWithId (id: int): World =  
    let dbContext = dataContext ()
    singleQueryFromWorld dbContext id

let worldForRandomId (random: Random): World =  
    let dbContext = dataContext ()
    singleRandomQueryFromWorld dbContext random

let multipleRandomWorlds (random: Random) (numberOfQueries: int): Async<array<World>> = 
    Async.Parallel [ for i in 1..numberOfQueries -> 
                                    async { 
                                        let dbContext = dataContext ()
                                        return singleRandomQueryFromWorld dbContext random
                                    }]

let updateWorldWithId (id: int) (update: int): World = updateSingleWorldRecord (dataContext ()) id update

let updateMultipleRandomWorldsWithRandomValues (random: Random) (numberOfQueries: int): Async<array<World>> =
    Async.Parallel [ for i in 1..numberOfQueries -> 
                                    async { 
                                        let dbContext = dataContext ()
                                        return updateRandomWorldRecordWithRandomValue dbContext random
                                    }]