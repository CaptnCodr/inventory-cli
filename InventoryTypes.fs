namespace Inventory

open FSharp.Data

module InventoryTypes =

    type Inventory = CsvProvider<"./Data/sample.csv", ResolutionFolder=__SOURCE_DIRECTORY__>

    type InventoryItem = { Ean: string; Description: string; Quantity: int }

