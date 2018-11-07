namespace Lambda.Cms 

module Core = 
    open System
    type GetUtcDate = unit -> DateTime
    type IdGenerator = unit -> Guid
    
    let internal _getUtcDate = fun () -> DateTime.UtcNow
    let getUtcDate = _getUtcDate

//module Core =
//    open Microsoft.Azure
//    open Microsoft.WindowsAzure.Storage
//    
//    type Filter =
//        | CategoryFilter of Slug
//        | IncludeDraft
//
//    type Page = 
//        { 
//            Index : int
//            Size : int
//        }
//
//    type DbGetDocumentsAsync = (Page * list<Filter>) -> Async<Document list>
//    type DbGetDocumentAsync = Slug -> Async<Document>  
//    
//    
//    let getSetting (name:string) =
//        name
//
//
//    let createContainerAsync containerName = async {
//            let connectionString = getSetting "AzureStorageAccount"
//            let storageAccount = CloudStorageAccount.Parse(connectionString)
//            let blobClient = storageAccount.CreateCloudBlobClient()
//            let container = blobClient.GetContainerReference(containerName)
//            let! _ = container.CreateIfNotExistsAsync() |> Async.AwaitTask 
//            return container
//        }