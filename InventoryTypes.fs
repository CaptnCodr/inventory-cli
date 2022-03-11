namespace Inventory

open FSharp.Data

module InventoryTypes =

    type Inventory = CsvProvider<"./Data/sample.csv", ResolutionFolder=__SOURCE_DIRECTORY__>

    type Inventoryitem = { Ean: string; Description: string; Quantity: int }

