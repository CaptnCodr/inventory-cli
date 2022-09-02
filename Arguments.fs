namespace Inventory

open Argu
open Resources

module Arguments =

    type InventoryItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Ean of string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-q")>] Quantity of int option
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-desc")>] Description of string option
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-u")>] Unit of string option

        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Ean _ -> Resource.Arguments_EAN.ResourceString
                | Quantity _ -> Resource.Arguments_Quantity.ResourceString
                | Description _ -> Resource.Arguments_Description.ResourceString
                | Unit _ -> Resource.Arguments_Unit.ResourceString

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
                | Add _ -> Resource.Arguments_ItemAdd.ResourceString
                | Edit _ -> Resource.Arguments_ItemEdit.ResourceString
                | Delete _ -> Resource.Arguments_ItemDelete.ResourceString
                | Increase _ -> Resource.Arguments_ItemIncrease.ResourceString
                | Decrease _ -> Resource.Arguments_ItemDecrease.ResourceString
                | List -> Resource.Arguments_ItemList.ResourceString

    type TagItemArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-e")>] Ean of string
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-n")>] Name of string
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Ean _ -> Resource.Arguments_EAN.ResourceString
                | Name _ -> Resource.Arguments_Name.ResourceString

    type TagArgs =
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-a")>] Add of ParseResults<TagItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-r")>] Remove of ParseResults<TagItemArgs>
        | [<CliPrefix(CliPrefix.None); AltCommandLine("-l")>] List of tag: string option
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Add _ -> Resource.Arguments_TagAdd.ResourceString
                | Remove _ -> Resource.Arguments_TagRemove.ResourceString
                | List _ -> Resource.Arguments_TagList.ResourceString

    type SettingsArgs =
        | [<CliPrefix(CliPrefix.None)>] SetPath of string
        | [<CliPrefix(CliPrefix.None)>] GetPath

        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | SetPath _ -> Resource.Arguments_SetPath.ResourceString
                | GetPath -> Resource.Arguments_GetPath.ResourceString

    [<DisableHelpFlags>]
    type CliArguments =
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-i")>] Item of ParseResults<ItemArgs>
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-t")>] Tag of ParseResults<TagArgs>
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-s")>] Settings of ParseResults<SettingsArgs>
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-v")>] Version
        | [<CliPrefix(CliPrefix.None);AltCommandLine("-h")>] Help
        
        interface IArgParserTemplate with
            member this.Usage =
                match this with 
                | Item _ -> Resource.Command_Item.ResourceString
                | Tag _ ->  Resource.Command_Tag.ResourceString
                | Settings _ -> Resource.Command_Settings.ResourceString
                | Version -> Resource.Command_Version.ResourceString
                | Help -> Resource.Command_Help.ResourceString
