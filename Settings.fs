namespace Inventory

open System

module Settings =

    [<Literal>]
    let InventoryKey = "Inventory_Path"

    let setInventoryPath path =
        Environment.SetEnvironmentVariable(InventoryKey, path, EnvironmentVariableTarget.User)

    let getInventoryPath () =
        (Environment.GetEnvironmentVariable(InventoryKey, EnvironmentVariableTarget.User) : string)
        |> Option.ofObj 
        |> Option.defaultValue ""