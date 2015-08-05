module Benchmarks.WebSharper.SqlProvider.Fortunes.View

open Benchmarks.WebSharper.SqlProvider.Fortunes.Types
open Benchmarks.WebSharper.SqlProvider.Routes
open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Content
open WebSharper.Html.Server

type ContentForTemplate = { Doctype: string; Title : string; Body : list<Element> }

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

let toPage (pageContent: ContentForTemplate) = Content.Page(Doctype = pageContent.Doctype, 
                                                            Title = pageContent.Title, 
                                                            Body = pageContent.Body)

//Requirement: must be UTF-8 with HTML starting with <!DOCTYPE html>
let toFortunePageContent (fortunes: seq<Fortune>): Async<Content<Endpoints>> = 
    let pageBody = fortunes |> toFortuneRows |> toTable headerRow 
    { Doctype = "<!DOCTYPE html>"; Title = "Fortunes"; Body = pageBody } |> toPage

