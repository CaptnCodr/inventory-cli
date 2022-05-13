namespace Inventory

open System
open Argu
open Arguments
open System.Reflection

module Program =
    
    let runCommands (parser: ArgumentParser<CliArguments>) (args: string array) =
        let parsingResult = parser.Parse args
        match parsingResult.GetAllResults() with
        | [ Item i ] -> 

            match i.GetAllResults() with 
            | [ ItemArgs.Add a ] -> 
                if a.Contains (InventoryItemArgs.Ean) then

                    let ean = a.GetResult(InventoryItemArgs.Ean)
                
                    let desc = 
                        a.TryGetResult(InventoryItemArgs.Description) 
                        |> Option.bind id 
                        |> (-&-) ""
                        ||> Option.defaultValue

                    let qty = 
                        a.TryGetResult (InventoryItemArgs.Quantity)
                        |> Option.bind id
                        |> (-&-) 0
                        ||> Option.defaultValue

                    { Ean = ean; Description = desc; Quantity = qty } |> ItemCommands.appendItem 
                else
                    parser.PrintUsage()

            | [ ItemArgs.Edit e ] -> 
                (e.GetResult(InventoryItemArgs.Ean), e.TryGetResult (InventoryItemArgs.Quantity) |> Option.bind id, e.TryGetResult(InventoryItemArgs.Description) |> Option.bind id) 
                |||> ItemCommands.editItem 

            | [ ItemArgs.Delete d ] -> 
                d |> ItemCommands.deleteItem

            | [ Increase e ] ->
                (+1, e) ||> ItemCommands.increaseDecreaseQty
                
            | [ Decrease e ] ->
                (-1, e) ||> ItemCommands.increaseDecreaseQty

            | [ ItemArgs.List ] ->
                ItemCommands.listItems()

            | _ -> parser.PrintUsage()

        | [ Tag t ] ->
            
            match t.GetAllResults() with 
            | [ TagArgs.Add a ] -> 
                (a.GetResult(TagItemArgs.Ean), a.GetResult(TagItemArgs.Name)) ||> TagCommands.addTagToItem

            | [ TagArgs.Remove r ] ->
                (r.GetResult(TagItemArgs.Ean), r.GetResult(TagItemArgs.Name)) ||> TagCommands.removeTagFromItem

            | [ TagArgs.List t ] -> 
                match t with 
                | Some t' -> t' |> TagCommands.listItemsWithTag
                | None -> TagCommands.listTags()

            | _ -> parser.PrintUsage()

        | [ Settings s ] ->
            
            match s.GetAllResults() with 
            | [ SettingsArgs.SetPath s ] -> 
                s |> Settings.setInventoryPath
                Resources.SettingCommand_PathSet.FormattedString(s)

            | [ SettingsArgs.GetPath ] -> Settings.getInventoryPath()

            | _ -> parser.PrintUsage()

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