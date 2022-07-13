namespace Inventory

open System
open System.Reflection
open System.Resources

module Resources =

    [<Literal>]
    let ResourceFile = "inventory-cli.Resources.Strings"

    type Resource =

        | ItemCommand_ItemAdded
        | ItemCommand_ItemsList
        | ItemCommand_ItemEdited
        | ItemCommand_IncreaseDecrease
        | ItemCommand_ItemDeleted
        
        | TagCommand_AddedTagToItem
        | TagCommand_ItemsWithTag
        | TagCommand_RemovedTagFromItem

        | SettingCommand_PathSet
        
        member this.ResourceString = 
            this.ToString() 
            |> ResourceManager(ResourceFile, Assembly.GetExecutingAssembly()).GetString

        member this.FormattedString ([<ParamArray>] args) =
            (this.ResourceString, args) |> String.Format
