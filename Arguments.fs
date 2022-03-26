namespace Inventory

open Argu

module Arguments =

    type InventoryItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Ean of string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-q")>] Quantity of int option
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-desc")>] Description of string option

        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Ean _ -> "EAN of that item"
                | Quantity _ -> "Quantity of that item."
                | Description _ -> "Description of that item"

    type ItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-a")>] Add of ParseResults<InventoryItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Edit of ParseResults<InventoryItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-d")>] Delete of ean: string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-inc")>] Increase of ean: string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-dec")>] Decrease of ean: string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-l")>] List
    
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Add _ -> "Adds an item."
                | Edit _ -> "Edits an item."
                | Delete _ -> "Deletes the product with the given EAN."
                | Increase _ -> "Increases product's quantity by 1."
                | Decrease _ -> "Decreases product's quantity by 1."

                | List -> "Show all items."

    type TagItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Ean of string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-n")>] Name of string
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Ean _ -> "EAN of that item."
                | Name _ -> "Name of tag."

    type TagArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-a")>] Add of ParseResults<TagItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-r")>] Remove of ParseResults<TagItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-l")>] List
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Add _ -> "Add Tag to item with EAN."
                | Remove _ -> "Remove Tag from item with EAN."
                | List -> "Show all existing tags."

    [<DisableHelpFlags>]
    type CliArguments =
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-i")>] Item of ParseResults<ItemArgs>

        | [<CliPrefix(CliPrefix.None);AltCommandLine("-t")>] Tag of ParseResults<TagArgs>

        | [<CliPrefix(CliPrefix.None);AltCommandLine("-v")>] Version
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Item _ -> "Command of item."
                | Tag _ ->  "Command of tags."
                | Version -> "Displays the version of 'Inventory'."
