module S3Access

open Amazon.S3
open Amazon.S3.Model
open TaskToAsync

let getAmazonS3Client = fun() -> new AmazonS3Client(Amazon.RegionEndpoint.EUWest1)

let getKeysAsync (s3ClientFactory : unit -> AmazonS3Client) marker bucketName = 
    async {
        let request = new ListObjectsRequest()
        request.BucketName <- bucketName
        request.Marker <- marker
        request.MaxKeys <- 1
        use client = s3ClientFactory()
        let! response = awaitTask (client.ListObjectsAsync(request))
        return (response.S3Objects |> Seq.map(fun s3Object -> s3Object.Key))
    }

