namespace Inventory

open System
open System.IO
open InventoryTypes
open FSharp.Data.Runtime

[<AutoOpen>]
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

    let createItem ean quantity description tags =
        InventoryTypes.Inventory.Row(ean, quantity, description, tags)

    let filterWithEan ean (provider: InventoryTypes.Inventory) =
        provider.Filter(fun item -> item.Ean = ean)

    let filterWithoutEan ean (provider: InventoryTypes.Inventory) =
        provider.Filter(fun item -> item.Ean <> ean)

    let filterItemSaveWithNew ean (item: InventoryTypes.Inventory.Row) =
        loadInventory()
        |> filterWithoutEan ean
        |> addItem item
        |> saveInventory

    let filterItemWithMatchingEan (ean: string) =
        (loadInventory() |> filterWithEan ean).Rows |> Seq.head
    
    module ItemCommands = 

        let listItems () =
            loadInventory().Rows
            |> Seq.map (fun r -> $"{r.Qty} of {r.Description} ({r.Ean}) with tags: {r.Tags}")
            |> String.concat Environment.NewLine

        let appendItem (data: InventoryItem) =
            let flattened = 
                (loadInventory() |> filterWithEan data.Ean).Rows 
                |> Seq.toArray 
                |> Array.append [| InventoryTypes.Inventory.Row(data.Ean, data.Quantity, data.Description, "") |]

            createItem flattened.[0].Ean (flattened |> Array.map (fun r -> r.Qty) |> Array.sum) flattened.[0].Description flattened.[0].Tags
            |> fun i -> (data.Ean, i) 
            ||> filterItemSaveWithNew

            $"{data.Ean} - {data.Quantity} - {data.Description} added!"

        let editItem (ean: string) (qty: int option) (description: string option) =
            let item' = ean |> filterItemWithMatchingEan
        
            createItem item'.Ean (qty |> Option.defaultValue item'.Qty) (description |> Option.defaultValue item'.Description) item'.Tags
            |> fun i -> (item'.Ean, i) 
            ||> filterItemSaveWithNew

            $"{item'.Qty} of {item'.Description} with EAN: {item'.Ean} edited!"

        let deleteItem (ean: string) =
            let item' = ean |> filterItemWithMatchingEan
        
            loadInventory()
            |> filterWithoutEan item'.Ean
            |> saveInventory

            $"{item'.Description} with EAN: {item'.Ean} deleted!"

        let increaseDecreaseQty (change: int) (ean: string) =
            let item' = ean |> filterItemWithMatchingEan
        
            createItem item'.Ean (item'.Qty + change) item'.Description item'.Tags
            |> fun i -> (item'.Ean, i) 
            ||> filterItemSaveWithNew
        
            $"Item {item'.Ean} changed by {change}!"
       
    module TagCommands =
        
        let listTags () =
            loadInventory()
            |> fun i -> i.Rows
            |> Seq.map (fun r -> r.Tags.Split(","))
            |> Seq.collect id
            |> Seq.distinct
            |> String.concat Environment.NewLine

        let addTagToItem (ean: string) (tag: string) =
            let item = ean |> filterItemWithMatchingEan 

            let itemTags = 
                item.Tags.Split(",") 
                |> Array.filter (fun i -> i <> "")
                |> Array.append [| tag |]
                |> Array.distinct
                |> String.concat ","

            createItem item.Ean item.Qty item.Description itemTags
            |> fun i -> (item.Ean, i) 
            ||> filterItemSaveWithNew

            $"Tag {tag} added to item {item.Ean}."

        let removeTagFromItem (ean: string) (tag: string) =
            let item = ean |> filterItemWithMatchingEan 

            let itemTags =  
                item.Tags.Split(",") 
                |> Array.filter (fun i -> i <> "")
                |> Array.filter (fun t -> t <> tag)
                |> String.concat ","
            
            createItem item.Ean item.Qty item.Description itemTags
            |> fun i -> (item.Ean, i) 
            ||> filterItemSaveWithNew

            $"Tag {tag} removed from item {item.Ean}."
