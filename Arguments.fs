namespace Inventory

open Argu

module Arguments =

    type AddItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Ean of string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-desc")>] Description of string option
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-q")>] Quantity of int option
    
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Ean _ -> "EAN of that item"
                | Description _ -> "Description of that item"
                | Quantity _ -> "Quantity of that item."

    type ItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-a")>] Add of ParseResults<AddItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-d")>] Delete of ean: string
    
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Add _ -> "Adds an item"
                | Delete _ -> "Deletes the product with the given EAN."

    [<DisableHelpFlags>]
    type CliArguments =
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-i")>] Item of ParseResults<ItemArgs>
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-is")>] Items

        | [<CliPrefix(CliPrefix.None);AltCommandLine("-v")>] Version
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Item _ -> "Command of item."
                | Items -> "Shows all items."
                | Version -> "Displays the version of 'Inventory'."

