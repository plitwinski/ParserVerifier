open System
open Estates.ParserVerifier

[<EntryPoint>]
let main argv =
    Handler.invoke { 
        bucket = "bucket"
        locations = [| "folder1"; "folder2" |]
        sender = "sender@sender.com"
        recipients = [| "recipient1@recipient.com"; "recipient2@recipient.com" |]
    } |> ignore
    Console.ReadKey() |> ignore
    0
