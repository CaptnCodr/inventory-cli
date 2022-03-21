namespace Inventory

open System
open System.IO
open InventoryTypes

module FileAccess =
    
    let private directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    [<Literal>]
    let private filename = "inventory.csv"

    let fullPath = Path.Combine(directory, filename)
    
    let getAllItems () =
        fullPath
        |> InventoryTypes.Inventory.Load
        |> fun i -> i.Rows
        |> Seq.map (fun r -> $"{r.Qty} of {r.Description} ({r.Ean})")
        |> String.concat Environment.NewLine

    let appendItem (data: InventoryItem) =
        let items' = fullPath |> InventoryTypes.Inventory.Load |> fun file -> file.Filter(fun item -> item.Ean = data.Ean).Rows
        let flattened = items' |> Seq.toArray |> Array.append [| InventoryTypes.Inventory.Row(data.Ean, data.Quantity, data.Description) |]

        let newItem = InventoryTypes.Inventory.Row(flattened.[0].Ean, flattened |> Array.map (fun r -> r.Qty) |> Array.sum, flattened.[0].Description)

        fullPath
        |> InventoryTypes.Inventory.Load 
        |> fun file -> file.Filter (fun t -> t.Ean <> data.Ean)
        |> fun i -> i.Append [ newItem ] 
        |> fun mod' -> mod'.Save fullPath

        $"{data.Ean} - {data.Quantity} - {data.Description} added!"

    let deleteItem (ean: string) =
        let item' = fullPath |> InventoryTypes.Inventory.Load |> fun file -> file.Filter(fun item -> item.Ean = ean).Rows |> Seq.head
        
        fullPath 
        |> InventoryTypes.Inventory.Load
        |> fun file -> file.Filter(fun item -> item <> item')
        |> fun mod' -> mod'.Save fullPath

        $"{item'.Description} with EAN: {item'.Ean} deleted!"