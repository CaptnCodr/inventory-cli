namespace Inventory

open System
open Resources
open FSharp.Data
open FSharp.Data.Runtime

type Inventory = CsvProvider<"./Data/sample.csv", Separators=";", ResolutionFolder=__SOURCE_DIRECTORY__>

type InventoryItem = { Ean: string; Description: string; Quantity: int }

[<AutoOpen>]
module Inventory =
    
    /// <summary>
    /// Tupelizes two values.
    /// </summary>
    /// <param name="f">First element in Tuple.</param>
    /// <param name="s">Second element in Tuple.</param>
    let inline (-&-) f s = (f, s)
    
    /// <summary>
    /// Splits the quadruple and passes it into the given function.
    /// </summary>
    /// <param name="q">Tuple of 4 elements (quadruple).</param>
    /// <param name="f">Function that takes 4 arguments.</param>
    let inline (||||>) q f = 
        let (a, b, c, d) = q
        f a b c d

    let loadInventory () =
        Settings.getInventoryPath() |> Inventory.Load

    let saveInventory (inv: CsvFile<'a>) =
        Settings.getInventoryPath() |> inv.Save

    let addItem (item: Inventory.Row) (inv: CsvFile<Inventory.Row>) =
        inv.Append [ item ]

    let createItem ean quantity description tags =
        Inventory.Row(ean, quantity, description, tags)

    let filterWithEan ean (provider: Inventory) =
        provider.Filter(fun item -> item.Ean = ean)

    let filterWithoutEan ean (provider: Inventory) =
        provider.Filter(fun item -> item.Ean <> ean)

    let filterItemSaveWithNew ean (item: Inventory.Row) =
        loadInventory()
        |> filterWithoutEan ean
        |> addItem item
        |> saveInventory

    let filterItemWithMatchingEan (ean: string) =
        loadInventory() 
        |> filterWithEan ean
        |> fun r -> r.Rows 
        |> Seq.head
    
    module ItemCommands = 

        let appendItem (data: InventoryItem) =
            let flattened = 
                (loadInventory() |> filterWithEan data.Ean).Rows 
                |> Seq.toArray 
                |> Array.append [| Inventory.Row(data.Ean, data.Quantity, data.Description, "") |]

            (flattened.[0].Ean, (flattened |> Array.map (fun r -> r.Qty) |> Array.sum), flattened.[0].Description, flattened.[0].Tags)
            ||||> createItem
            |> (-&-) data.Ean
            ||> filterItemSaveWithNew

            ItemCommand_ItemAdded.FormattedString(data.Ean, data.Quantity, data.Description)

        let listItems () =
            loadInventory().Rows
            |> Seq.map (fun r -> ItemCommand_ItemsList.FormattedString(r.Qty, r.Description, r.Ean, r.Tags))
            |> String.concat Environment.NewLine

        let editItem (ean: string) (qty: int option) (description: string option) =
            let item = ean |> filterItemWithMatchingEan

            (item.Ean, (qty |> Option.defaultValue item.Qty), (description |> Option.defaultValue item.Description), item.Tags)
            ||||> createItem
            |>    (-&-) item.Ean
            ||>   filterItemSaveWithNew

            ItemCommand_ItemEdited.FormattedString(item.Qty, item.Description, item.Ean)
            
        let increaseDecreaseQty (change: int) (ean: string) =
            let item = ean |> filterItemWithMatchingEan

            (item.Ean, (item.Qty + change), item.Description, item.Tags) 
            ||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            ItemCommand_IncreaseDecrease.FormattedString(item.Ean, change)

        let deleteItem (ean: string) =
            let item = ean |> filterItemWithMatchingEan

            loadInventory()
            |> filterWithoutEan item.Ean
            |> saveInventory

            ItemCommand_ItemDeleted.FormattedString(item.Description, item.Ean)

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

            (item.Ean, item.Qty, item.Description, itemTags)
            ||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            TagCommand_AddedTagToItem.FormattedString(tag, item.Ean)

        let listItemsWithTag (tag: string) =
            loadInventory()
            |> fun r -> r.Rows
            |> Seq.filter (fun i -> i.Tags.Split(",") 
                                    |> Array.exists (fun e -> e = tag))
            |> Seq.map (fun i -> TagCommand_ItemsWithTag.FormattedString(i.Ean, i.Qty, i.Description))
            |> String.concat Environment.NewLine

        let removeTagFromItem (ean: string) (tag: string) =
            let item = ean |> filterItemWithMatchingEan 

            let itemTags =  
                item.Tags.Split(",") 
                |> Array.filter (fun i -> i <> "")
                |> Array.filter (fun t -> t <> tag)
                |> String.concat ","
            
            (item.Ean, item.Qty, item.Description, itemTags)
            ||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            TagCommand_RemovedTagFromItem.FormattedString(tag, item.Ean)
