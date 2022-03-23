namespace Inventory

open System
open System.IO
open InventoryTypes
open FSharp.Data.Runtime

module InventoryCommands =
    
    let private directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    [<Literal>]
    let private filename = "inventory.csv"

    let fullPath = Path.Combine(directory, filename)

    let loadInventory () =
        fullPath |> InventoryTypes.Inventory.Load

    let saveInventory (inv: CsvFile<'a>) =
        inv.Save fullPath

    let addItem (item: InventoryTypes.Inventory.Row) (inv: CsvFile<InventoryTypes.Inventory.Row>) =
        inv.Append [ item ]

    let filterWithEan (ean: string) (provider: InventoryTypes.Inventory) =
        provider.Filter(fun item -> item.Ean = ean)

    let filterWithoutEan (ean: string) (provider: InventoryTypes.Inventory) =
        provider.Filter(fun item -> item.Ean <> ean)

    let filterItemWithMatchingEan (ean: string) =
        (loadInventory() |> filterWithEan ean).Rows |> Seq.head
    
    let getAllItems () =
        loadInventory()
        |> fun i -> i.Rows
        |> Seq.map (fun r -> $"{r.Qty} of {r.Description} ({r.Ean})")
        |> String.concat Environment.NewLine

    let appendItem (data: InventoryItem) =
        let flattened = 
            (loadInventory() |> filterWithEan data.Ean).Rows 
            |> Seq.toArray 
            |> Array.append [| InventoryTypes.Inventory.Row(data.Ean, data.Quantity, data.Description) |]

        let newItem = InventoryTypes.Inventory.Row(flattened.[0].Ean, flattened |> Array.map (fun r -> r.Qty) |> Array.sum, flattened.[0].Description)

        loadInventory()
        |> filterWithoutEan data.Ean
        |> addItem newItem
        |> saveInventory

        $"{data.Ean} - {data.Quantity} - {data.Description} added!"

    let editItem (ean: string) (qty: int option) (description: string option) =
        let item' = ean |> filterItemWithMatchingEan
        
        let newItem = InventoryTypes.Inventory.Row(item'.Ean, qty |> Option.defaultValue item'.Qty, description |> Option.defaultValue item'.Description)

        loadInventory()
        |> filterWithoutEan item'.Ean
        |> addItem newItem
        |> saveInventory

        $"{item'.Qty} of {item'.Description} with EAN: {item'.Ean} edited!"

    let deleteItem (ean: string) =
        let item' = ean |> filterItemWithMatchingEan
        
        loadInventory()
        |> filterWithoutEan item'.Ean
        |> saveInventory

        $"{item'.Description} with EAN: {item'.Ean} deleted!"

    let increaseDecreaseQty (change: int) (ean: string) =
        let item' = ean |> filterItemWithMatchingEan
        
        let newItem = InventoryTypes.Inventory.Row(item'.Ean, item'.Qty + change, item'.Description)

        loadInventory()
        |> filterWithoutEan item'.Ean
        |> addItem newItem
        |> saveInventory
        
        $"Item {item'.Ean} changed by {change}!"
        