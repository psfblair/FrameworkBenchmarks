module Benchmarks.WebSharper.SqlProvider.Hello

open WebSharper
open WebSharper.Sitelets

type Hello = { message: string }

let private helloMessage: string = "Hello, World!"
let private toHello (message: string): Hello = { Hello.message = message } 

//Not allowed to store entire response in a preallocated buffer, so we make this a function.
let private toPlaintextResponse (msg: string) : Http.Response = { 
            Status    = Http.Status.Ok 
            Headers   = [Http.Header.Custom "Content-Type" "text/plain"] 
            WriteBody = fun stream -> 
                use w = new System.IO.StreamWriter(stream) 
                w.Write(msg) 
        }

let plaintextContent context = async { return helloMessage |> toPlaintextResponse  }

let jsonContent context = async { return helloMessage |> toHello } 
