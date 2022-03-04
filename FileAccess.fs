namespace Inventory

open System
open System.IO

module FileAccess =

    let private directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    [<Literal>]
    let private filename = "inventory.json"

    let checkInventoryFile =
        let file = directory + filename
        if File.Exists file then
            use stream = new StreamReader (file)

            stream.ReadToEnd() |> ignore
        else 
            File.Create(file) |> ignore


