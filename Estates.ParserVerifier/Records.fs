module Records

type RequestContext = { 
    bucket: string
    locations: string array
    sender: string
    recipients: string array
}

type VerificationResult = { 
    missingLocations: string array
    validationPassed: bool
}