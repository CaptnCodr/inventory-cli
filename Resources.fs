namespace Inventory

open System.Reflection
open System.Resources

module Resources =

    [<Literal>]
    let ResourceFile = "inventory.Resources.Strings"

    type Resource =

        | ItemCommand_ItemAdded
        | ItemCommand_ItemsList
        | ItemCommand_ItemEdited
        | ItemCommand_IncreaseDecrease
        | ItemCommand_ItemDeleted
        
        | TagCommand_AddedTagToItem
        | TagCommand_ItemsWithTag
        | TagCommand_RemovedTagFromItem
        
        member this.ResourceString = 
            this.ToString() 
            |> ResourceManager(ResourceFile, Assembly.GetExecutingAssembly()).GetString

        member this.FormattedString ([<System.ParamArray>] args) =
            this.ResourceString
            |> fun s -> (s, args) |> System.String.Format
