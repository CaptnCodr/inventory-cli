namespace Inventory

open System
open Argu
open Arguments
open FileAccess

module Program =

    
    let runCommands (parser: ArgumentParser<CliArguments>) (args: string array) =
        let parsingResult = parser.Parse args
        match parsingResult.GetAllResults() with
        | [ Item _ ] -> "Item"
        | [ Items ] -> "Items"

        | _ -> FileAccess.readFile
    
    [<EntryPoint>]
    let main ([<ParamArray>] argv: string[]): int =

        try 
            (ArgumentParser.Create<CliArguments>(), argv)
            ||> runCommands 
            |> printfn "%s"

        with 
        | ex -> eprintfn $"{ex.Message}"

        0