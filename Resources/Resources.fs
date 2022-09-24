namespace Inventory

open Extensions.ResourceExt

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

        | Arguments_EAN
        | Arguments_Name
        | Arguments_Quantity
        | Arguments_Description
        | Arguments_Unit

        | Arguments_ItemAdd
        | Arguments_ItemEdit
        | Arguments_ItemDelete
        | Arguments_ItemIncrease
        | Arguments_ItemDecrease
        | Arguments_ItemList

        | Arguments_TagAdd
        | Arguments_TagRemove
        | Arguments_TagList

        | Arguments_SetPath
        | Arguments_GetPath

        | Command_Item
        | Command_Tag
        | Command_Settings
        | Command_Version
        | Command_Help

        member this.ResourceString = ResourceFile |> resourceManager |> getResourceString this

        member this.FormattedString([<System.ParamArray>] args) =
            ResourceFile |> resourceManager |> getFormattedString this args
