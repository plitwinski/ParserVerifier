namespace Estates.ParserVerifier
open Amazon.Lambda.Core

[<assembly:LambdaSerializer(typeof<Amazon.Lambda.Serialization.Json.JsonSerializer>)>]
do ()

module Handler =
    open Records
    open Verification
    open SesAccess
    open Notifications

    let private logVerificationResult result = 
        let locations = 
            if (result.missingLocations.Length = 0) then
                "NO MISSING LOCATIONS"
            else
                result.missingLocations |> Seq.reduce(fun acc next -> acc + ", " + next)

        printfn "Validation passed: %b" result.validationPassed
        printfn "Missing locations: %s" locations

    let private sendNotificationsAsync (request:RequestContext) (verificationResult:VerificationResult) =
        async {
            if not verificationResult.validationPassed then
                let sendMessageAsync = sendEmailsAsync getAmazonSesClient
                let! isSuccessful = sendWarningAboutVerificationFailure sendMessageAsync 
                                                                  request.sender 
                                                                  request.recipients 
                                                                  verificationResult.missingLocations
                if not isSuccessful then
                    printfn "Unable to send an email verification failed email"
        }


    let invoke (request:RequestContext) =
        let verificationResult = verifyBucket request
        logVerificationResult verificationResult
        sendNotificationsAsync request verificationResult |> Async.RunSynchronously

        
