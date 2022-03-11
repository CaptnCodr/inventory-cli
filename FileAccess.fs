namespace Inventory

open System
open System.IO
open FSharp.Data
open InventoryTypes

module FileAccess =
    
    let private directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    [<Literal>]
    let private filename = "inventory.csv"

    //let checkInventoryFile =
    //    let file = Path.Combine(directory, filename)
    //    if File.Exists file then
    //        use stream = new StreamReader (file)

    //        stream.ReadToEnd() |> ignore
    //    else 
    //        File.Create(file) |> ignore

    let writeFile (data: Inventoryitem) =
        Path.Combine(directory, filename) 
        |> File.OpenRead 
        |> InventoryTypes.Inventory.Load
        |> fun file -> file.Append [ Inventory.Row(data.Ean, data.Quantity, data.Description) ]
        $"{data.Ean} - {data.Quantity} - {data.Description} added!"

    let readFile =
        Path.Combine(directory, filename)
        |> File.OpenRead 
        |> InventoryTypes.Inventory.Load
        |> fun i -> i.Rows
        |> Seq.map (fun r -> $"{r.Ean}, {r.Qty} pcs - {r.Description}")
        |> String.concat Environment.NewLine