module Notifications

open System

let sendWarningAboutVerificationFailure sendMessageAsync sender (recipients: seq<string>) (failedLocations: seq<string>) = 
    async {
        let subject = "Estates parsing verification failed"

        let addDot = Environment.NewLine + "● "

        let concentratedLocations = failedLocations 
                                    |> Seq.reduce(fun acc next -> acc + addDot + next)

        let content = "Estates verification failed for following locations:" + addDot + concentratedLocations
        let! result = sendMessageAsync sender recipients subject content
        return result
    }
