namespace Inventory

open System

module Settings =

    [<Literal>]
    let InventoryPath = "Inventory_Path"

    let setInventoryPath path =
        Environment.SetEnvironmentVariable(InventoryPath, path, EnvironmentVariableTarget.User)

    let getInventoryPath () =
        (Environment.GetEnvironmentVariable(InventoryPath, EnvironmentVariableTarget.User): string)
        |> Option.ofObj
        |> Option.defaultValue ""
