namespace Inventory

open System
open Resources
open FSharp.Data
open FSharp.Data.Runtime

type Inventory = CsvProvider<"./Data/sample.csv", Separators=";", ResolutionFolder=__SOURCE_DIRECTORY__>

type InventoryItem =
    { Ean: string
      Description: string
      Quantity: int
      Unit: string
      Price: decimal }

[<AutoOpen>]
module Inventory =

    /// <summary>
    /// Tupelizes two values.
    /// </summary>
    /// <param name="f">First element in Tuple.</param>
    /// <param name="s">Second element in Tuple.</param>
    let inline (-&-) f s = (f, s)

    /// <summary>
    /// Splits the quintuple and passes it into the given function.
    /// </summary>
    /// <param name="q">Tuple of 5 elements (quintuple).</param>
    /// <param name="f">Function that takes 5 arguments.</param>
    let inline (|||||>) q f =
        let (a, b, c, d, e) = q
        f a b c d e

    /// <summary>
    /// Splits the sextuple and passes it into the given function.
    /// </summary>
    /// <param name="q">Tuple of 6 elements (sextuple).</param>
    /// <param name="f">Function that takes 6 arguments.</param>
    let inline (||||||>) q f =
        let (a, b, c, d, e, g) = q
        f a b c d e g

    let loadInventory () =
        Settings.getInventoryPath () |> Inventory.Load

    let saveInventory (inv: CsvFile<Inventory.Row>) =
        Settings.getInventoryPath () |> inv.Save

    let addItem (item: Inventory.Row) (inv: CsvFile<Inventory.Row>) = inv.Append [ item ]

    let createItem ean quantity description tags unit price =
        Inventory.Row(ean, quantity, description, tags, unit, price)

    let filterWithEan ean (provider: Inventory) =
        provider.Filter(fun item -> item.Ean = ean)

    let filterWithoutEan ean (provider: Inventory) =
        provider.Filter(fun item -> item.Ean <> ean)

    let filterItemSaveWithNew ean (item: Inventory.Row) =
        loadInventory () |> filterWithoutEan ean |> addItem item |> saveInventory

    let filterItemWithMatchingEan (ean: string) =
        loadInventory () |> filterWithEan ean |> (fun r -> r.Rows) |> Seq.head

    module ItemCommands =

        let appendItem (data: InventoryItem) =
            let flattened =
                (loadInventory () |> filterWithEan data.Ean).Rows
                |> Seq.toArray
                |> Array.append
                    [| Inventory.Row(data.Ean, data.Quantity, data.Description, "", data.Unit, data.Price) |]

            (flattened.[0].Ean,
             (flattened |> Array.map (fun r -> r.Qty) |> Array.sum),
             flattened.[0].Description,
             flattened.[0].Tags,
             flattened.[0].Unit,
             flattened.[0].Price)
            ||||||> createItem
            |> (-&-) data.Ean
            ||> filterItemSaveWithNew

            ItemCommand_ItemAdded.FormattedString(data.Ean, data.Quantity, data.Description)

        let listItems () =
            loadInventory().Rows
            |> Seq.mapi (fun i r ->
                ItemCommand_ItemsList.FormattedString(r.Qty, r.Description, r.Ean, r.Tags, r.Unit, r.Price, i))
            |> String.concat Environment.NewLine

        let editItem
            (ean: string)
            (qty: int option)
            (description: string option)
            (unit: string option)
            (price: decimal option)
            =
            let item = ean |> filterItemWithMatchingEan

            (item.Ean,
             (qty |> Option.defaultValue item.Qty),
             (description |> Option.defaultValue item.Description),
             item.Tags,
             (unit |> Option.defaultValue item.Unit),
             (price |> Option.defaultValue item.Price))
            ||||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            ItemCommand_ItemEdited.FormattedString(item.Qty, item.Description, item.Ean)

        let increaseDecreaseQty (change: int) (ean: string) =
            let item = ean |> filterItemWithMatchingEan

            (item.Ean, (item.Qty + change), item.Description, item.Tags, item.Unit, item.Price)
            ||||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            ItemCommand_IncreaseDecrease.FormattedString(item.Ean, change)

        let deleteItem (ean: string) =
            let item = ean |> filterItemWithMatchingEan

            loadInventory () |> filterWithoutEan item.Ean |> saveInventory

            ItemCommand_ItemDeleted.FormattedString(item.Description, item.Ean)

    module TagCommands =

        let listTags () =
            loadInventory ()
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

            (item.Ean, item.Qty, item.Description, itemTags, item.Unit, item.Price)
            ||||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            TagCommand_AddedTagToItem.FormattedString(tag, item.Ean)

        let listItemsWithTag (tag: string) =
            loadInventory ()
            |> fun r -> r.Rows
            |> Seq.filter (fun i -> i.Tags.Split(",") |> Array.exists (fun e -> e = tag))
            |> Seq.map (fun i -> TagCommand_ItemsWithTag.FormattedString(i.Ean, i.Qty, i.Description))
            |> String.concat Environment.NewLine

        let removeTagFromItem (ean: string) (tag: string) =
            let item = ean |> filterItemWithMatchingEan

            let itemTags =
                item.Tags.Split(",")
                |> Array.filter (fun i -> i <> "")
                |> Array.filter (fun t -> t <> tag)
                |> String.concat ","

            (item.Ean, item.Qty, item.Description, itemTags, item.Unit, item.Price)
            ||||||> createItem
            |> (-&-) item.Ean
            ||> filterItemSaveWithNew

            TagCommand_RemovedTagFromItem.FormattedString(tag, item.Ean)
