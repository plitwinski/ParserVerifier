module Verification

open System
open Records
open S3Access
open FSharp.Control

let private jsGetTime (now: DateTime) : int64 = 
        let startTime = new DateTime(1970,1,1, 0, 0, 0, DateTimeKind.Utc)
        let diff = now.ToUniversalTime() - startTime
        int64(diff.TotalMilliseconds + 0.5)

let private createMarker (currentUtcTime : DateTime) (toTimeName: DateTime -> int64) location  =
            let name = toTimeName (currentUtcTime.AddHours(-12.0))
            location + "/" + name.ToString()

let private containsKey (foundKeys: Async<seq<string>>) = 
        async {   
            let! foundKeysResult = foundKeys
            let items = foundKeysResult
                        |> Seq.filter(fun key -> key.EndsWith(".csv"))
                        |> Seq.toArray
            return items.Length > 0
        }

let verifyBucket (request:RequestContext) =
    let toMarker = fun location ->  createMarker DateTime.UtcNow jsGetTime location
    let getKeysForMarkerAsync = fun marker bucket -> getKeysAsync getAmazonS3Client marker bucket

    let filterLocationsAsync location = async {
        let! hasKey = (containsKey (getKeysForMarkerAsync (toMarker location) request.bucket))
        return not hasKey
    }

    let missingLocations = request.locations 
                                |> AsyncSeq.ofSeq
                                |> AsyncSeq.filterAsync(fun location -> filterLocationsAsync location)
                                |> AsyncSeq.toArray

    { missingLocations = missingLocations
      validationPassed = missingLocations.Length = 0 }

