namespace Benchmarks.WebSharper.SqlProvider

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.IO

open Owin
open Microsoft.Owin
open Microsoft.Owin.Logging
open WebSharper.Owin
open WebSharper

module Logging = 
    let logRequestBody = false
    let logResponseBody = false

    let simpleRequestKeys = [ "owin.RequestScheme";  "owin.RequestProtocol";  "owin.RequestMethod";  "owin.RequestPathBase";  "owin.RequestPath";    
                              "owin.RequestQueryString"; "server.RemoteIpAddress"; "server.RemotePort";  "server.LocalIpAddress";  "server.LocalPort"]
    let dictionaryRequestKeys =  ["owin.RequestHeaders"; "Microsoft.Owin.Cookies#dictionary"; "Microsoft.Owin.Query#dictionary"]

    let simpleResponseKeys =     ["owin.ResponseStatusCode"; "owin.RequestMethod"; "owin.RequestPath"]
    let dictionaryResponseKeys = ["owin.ResponseHeaders"]   

    type OwinLogger(next: AppFunc, appBuilder: Owin.IAppBuilder) =

        let logDictValue (logger: ILogger) (valueType: string) (dict: IDictionary<string,'a>) (key: string) = 
            match dict.TryGetValue(key) with
            | (true, value) -> logger.WriteInformation(String.Format("{0} - {1}: {2}", valueType, key, value))
            | _ -> () 

        let logDictValues (logger: ILogger) (env: Env) (key: string) = 
            match env.TryGetValue(key) with
            | (true, (:? IDictionary<string, string> as dictionary)) -> dictionary.Keys |> Seq.iter (logDictValue logger key dictionary)
            | _ -> () 
        
        let defaultValue value result = 
            match result with
            | (true, resultValue) -> resultValue.ToString()
            | _ -> value 

        let requestToString time (dict: IDictionary<string,'a>) = 
            let remoteIpAddress = dict.TryGetValue("server.RemoteIpAddress") |> defaultValue ""
            let remotePort = dict.TryGetValue("server.RemotePort")  |> defaultValue ""
            let protocol = dict.TryGetValue("owin.RequestProtocol") |> defaultValue ""
            let localIP = dict.TryGetValue("server.LocalIpAddress") |> defaultValue ""
            let localPort = dict.TryGetValue("server.LocalPort") |> defaultValue ""
            let requestMethod = dict.TryGetValue("owin.RequestMethod") |> defaultValue ""
            let requestPath = dict.TryGetValue("owin.RequestPath") |> defaultValue ""
            let requestQueryString = dict.TryGetValue("owin.RequestQueryString") |> defaultValue ""

            String.Format("{0} {1}:{2} {3} {4}:{5} {6} {7} {8}", 
                            time, remoteIpAddress, remotePort, protocol, localIP, localPort, 
                            requestMethod, requestPath, requestQueryString) 

        let logRequest (logger: ILogger) env = 
            requestToString DateTime.Now env |> logger.WriteInformation
            dictionaryRequestKeys |> Seq.iter (fun key -> logDictValues logger env key)

        let responseToString time (dict: IDictionary<string,'a>) = 
            let requestMethod = dict.TryGetValue("owin.RequestMethod") |> defaultValue ""
            let requestPath = dict.TryGetValue("owin.RequestPath") |> defaultValue ""
            let responseStatus = dict.TryGetValue("owin.ResponseStatusCode") |> defaultValue ""
            String.Format("{0} {1} {2} {3}", time, requestMethod, requestPath, responseStatus)

        let logResponse (logger: ILogger) env =
            responseToString DateTime.Now env |> logger.WriteInformation
            dictionaryResponseKeys |> Seq.iter (fun key -> logDictValues logger env key)
        
        member this.Invoke(env: Env) =               
            async {
                let logger = appBuilder.CreateLogger<OwinLogger>()
                let context = new OwinContext(env)
                let responseStream = context.Response.Body
                let responseBuffer = new MemoryStream() :> Stream

                logRequest logger env

                if logger.IsEnabled(Diagnostics.TraceEventType.Verbose) then
                    let requestStream = context.Request.Body
                    let buffer = new MemoryStream()
                    do! requestStream.CopyToAsync(buffer) |> Async.AwaitIAsyncResult |> Async.Ignore
                    let inputStreamReader = new StreamReader(buffer)
                    let! requestBody = inputStreamReader.ReadToEndAsync() |> Async.AwaitTask
                    logger.WriteVerbose("Request Body:")
                    logger.WriteVerbose(requestBody)
                    buffer.Seek(0L, SeekOrigin.Begin) |> ignore
                    do! buffer.CopyToAsync(requestStream) |> Async.AwaitIAsyncResult |> Async.Ignore
                    context.Response.Body <- responseBuffer

                do! next.Invoke(env) |> Async.AwaitIAsyncResult |> Async.Ignore

                logResponse logger env

                if logger.IsEnabled(Diagnostics.TraceEventType.Verbose) then
                    responseBuffer.Seek(0L, SeekOrigin.Begin) |> ignore
                    let outputStreamReader = new StreamReader(responseBuffer)
                    let! responseBody = outputStreamReader.ReadToEndAsync() |> Async.AwaitTask
                    logger.WriteVerbose("Response Body:")
                    logger.WriteVerbose(responseBody)
                    responseBuffer.Seek(0L, SeekOrigin.Begin) |> ignore
                    do! responseBuffer.CopyToAsync(responseStream) |> Async.AwaitIAsyncResult |> Async.Ignore

            } |> Async.Ignore |> Async.StartAsTask :> Task

    let logger: MiddlewareGenerator = 
        let loggerFunc (appBuilder: Owin.IAppBuilder) =
            let midFunc(next: AppFunc) = 
                let appFunc (env: Env) = (new OwinLogger(next, appBuilder)).Invoke(env)
                AppFunc(appFunc)
            MidFunc(midFunc)
        MiddlewareGenerator(loggerFunc)

