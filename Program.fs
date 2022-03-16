namespace Inventory

open System
open System.IO
open Argu
open Arguments
open InventoryTypes
open FileAccess

module Program =

    
    let runCommands (parser: ArgumentParser<CliArguments>) (args: string array) =
        let parsingResult = parser.Parse args
        match parsingResult.GetAllResults() with
        | [ Item i ] -> 
            match i.GetAllResults() with 
            | [ Add a ] -> 
                if a.Contains (AddItemArgs.Ean) then

                    let ean = a.GetResult(AddItemArgs.Ean)
                
                    let desc = 
                        a.TryGetResult(AddItemArgs.Description) 
                        |> Option.bind id 
                        |> fun d -> ("", d) 
                        ||> Option.defaultValue

                    let qty = 
                        a.TryGetResult (AddItemArgs.Quantity)
                        |> Option.bind id
                        |> fun q -> (0, q)
                        ||> Option.defaultValue

                    { Ean = ean; Description = desc; Quantity = qty } |> FileAccess.appendItem 
                else
                    parser.PrintUsage()
            | [ Delete d ] -> 
                d |> FileAccess.deleteItem

            | _ -> parser.PrintUsage()
        | [ Items ] -> FileAccess.getAllItems()

        | [ Version ] -> "0.0.0.0"
        | _ -> parser.PrintUsage()
    
    [<EntryPoint>]
    let main ([<ParamArray>] argv: string[]): int =

        try 
            (ArgumentParser.Create<CliArguments>(), argv)
            ||> runCommands 
            |> printfn "%s"

        with 
        | ex -> eprintfn $"{ex.Message}"

        0