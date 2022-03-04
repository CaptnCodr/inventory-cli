namespace Inventory

open System.IO

module FileAccess =

    [<Literal>]
    let private directory = @"C:\Users\const\"

    [<Literal>]
    let private filename = "inventory.json"

    let checkInventoryFile =
        let file = directory + filename
        if File.Exists file then
            use stream = new StreamReader (file)

            stream.ReadToEnd() |> ignore
        else 
            File.Create(file) |> ignore


