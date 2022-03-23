namespace Inventory

open System
open Argu
open Arguments
open InventoryTypes
open System.Reflection

module Program =
    
    let runCommands (parser: ArgumentParser<CliArguments>) (args: string array) =
        let parsingResult = parser.Parse args
        match parsingResult.GetAllResults() with
        | [ Item i ] -> 

            match i.GetAllResults() with 
            | [ Add a ] -> 
                if a.Contains (InventoryItemArgs.Ean) then

                    let ean = a.GetResult(InventoryItemArgs.Ean)
                
                    let desc = 
                        a.TryGetResult(InventoryItemArgs.Description) 
                        |> Option.bind id 
                        |> fun d -> ("", d) 
                        ||> Option.defaultValue

                    let qty = 
                        a.TryGetResult (InventoryItemArgs.Quantity)
                        |> Option.bind id
                        |> fun q -> (0, q)
                        ||> Option.defaultValue

                    { Ean = ean; Description = desc; Quantity = qty } |> InventoryCommands.appendItem 
                else
                    parser.PrintUsage()
            | [ Edit e ] -> 
                if e.Contains (InventoryItemArgs.Ean) then
                    (e.GetResult(InventoryItemArgs.Ean), e.TryGetResult (InventoryItemArgs.Quantity) |> Option.bind id, e.TryGetResult(InventoryItemArgs.Description) |> Option.bind id) 
                    |||> InventoryCommands.editItem 
                else
                    parser.PrintUsage()
            | [ Delete d ] -> 
                d |> InventoryCommands.deleteItem

            | [ Increase e ] ->
                e |> InventoryCommands.increaseDecreaseQty +1
                
            | [ Decrease e ] ->
                e |> InventoryCommands.increaseDecreaseQty -1

            | _ -> parser.PrintUsage()
        | [ Items ] -> InventoryCommands.getAllItems()

        | [ Version ] -> Assembly.GetExecutingAssembly().GetName().Version |> string
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