namespace Inventory

open Argu

module Arguments =

    type AddItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Ean of string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-d")>] Description of string option
    
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Ean _ -> "Adds an item"
                | Description _ -> "Adds an item"

    type ItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-a")>] Add of ParseResults<AddItemArgs>
    
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Add _ -> "Adds an item"

    [<DisableHelpFlags>]
    type CliArguments =
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-i")>] Item of ParseResults<ItemArgs>
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-is")>] Items
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Item _ -> "item"
                | Items -> ""

