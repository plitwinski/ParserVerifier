module SesAccess

open Amazon.SimpleEmail
open Amazon
open Amazon.SimpleEmail.Model
open TaskToAsync
open System.Net

let getAmazonSesClient = fun() -> new AmazonSimpleEmailServiceClient(RegionEndpoint.EUWest1)

let sendEmailsAsync (sesClientFactory : unit -> AmazonSimpleEmailServiceClient) sender (recipients: seq<string>) subject (content:string) =
        async {
            let body = new Body()
            body.Text <- new Content(content)
            body.Text.Charset <- "UTF-8"

            let request = new SendEmailRequest()
            request.Source <- sender
            request.Destination <- new Destination(ResizeArray<string> recipients)
            request.Message <- new Message(new Content(subject), body);

            use client = sesClientFactory()
            let! response = awaitTask(client.SendEmailAsync(request))

            return response.HttpStatusCode < HttpStatusCode.Ambiguous
        }

