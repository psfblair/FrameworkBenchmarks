module Benchmarks.WebSharper.SqlProvider.Data

open System
open FSharp.Data.Sql

[<Literal>] 
let compileTimeConnectionString = """
Host=127.0.0.1;
Port=5432;
Database=hello_world;
Username=benchmarkdbuser;
Password=benchmarkdbpass;
Pooling=true;MinPoolSize=5;
MaxPoolSize=20"""

type Db = SqlDataProvider<DatabaseVendor    = Common.DatabaseProviderTypes.POSTGRESQL, 
                          ConnectionString  = compileTimeConnectionString,
                          ResolutionPath    = "packages/Npgsql/lib/net45/",
                          UseOptionTypes    = true>

let runtimeConnectionString = 
    let fromEnvironment = System.Environment.GetEnvironmentVariable("BENCHMARK_DB_CONNECT_STRING")
    match fromEnvironment with
        | null -> compileTimeConnectionString
        | _    -> fromEnvironment

let dataContext = fun () -> Db.GetDataContext(runtimeConnectionString)