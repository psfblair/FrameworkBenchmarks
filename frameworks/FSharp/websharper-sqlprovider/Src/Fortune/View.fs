module Benchmarks.WebSharper.SqlProvider.Fortunes.View

open Benchmarks.WebSharper.SqlProvider.Fortunes.Types

open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Content
open WebSharper.Html.Server

type ContentForTemplate = { Title : string; Body : list<Element> }

let private headerRow: Element = TR [ TH [Text "id"] ; TH [Text "message"] ]

let private fortuneRow (fortune: Fortune): Element =
    let idString = fortune.id.ToString()
    TR [ 
        TD [Text idString]
        TD [Text fortune.message] 
    ]

let private toFortuneRows (fortunes: seq<Fortune>): list<Element> =
    fortunes |> Seq.map fortuneRow |> Seq.toList

let private toTable (headerRow: Element) (fortuneRows: list<Element>): list<Element> =
    [ Table (headerRow :: fortuneRows) ]

(*
// I wish we could do this, but the test rigorously enforces no spaces
// between the title tags and the title, so we'll use a template instead.
let private toPage (doctype: string) (title: string) (body: list<Element>) = 
    Warp.Page(Doctype = doctype, Title = title, Body = body)
*)


//Requirement: must be UTF-8 with template starting with <!DOCTYPE html>
let toFortunePageContent (fortunes: seq<Fortune>) = 
    let pageBody = fortunes |> toFortuneRows |> toTable headerRow 
    async { return { Title = "Fortunes"; Body = pageBody } }

let fortuneTemplate : Content.Template<ContentForTemplate> = 
    Content.Template<ContentForTemplate>("~/Src/Fortune/Fortunes.html")
            .With("title", fun x -> x.Title)
            .With("body",  fun x -> x.Body)
