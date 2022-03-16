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
        fullPath
        |> InventoryTypes.Inventory.Load 
        |> fun i -> i.Append [ Inventory.Row(data.Ean, data.Quantity, data.Description) ]
        |> fun mod' -> mod'.Save fullPath

        $"{data.Ean} - {data.Quantity} - {data.Description} added!"

    let deleteItem (ean: string) =
        let item' = fullPath |> InventoryTypes.Inventory.Load |> fun file -> file.Filter(fun item -> item.Ean = ean).Rows |> Seq.head
        
        fullPath 
        |> InventoryTypes.Inventory.Load
        |> fun file -> file.Filter(fun item -> item <> item')
        |> fun mod' -> mod'.Save fullPath

        $"{item'.Description} with EAN: {item'.Ean} deleted!"