namespace Lambda.Cms

open System
open Chessie
open Core

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module DraftChangeSet = 
    let create (idGenerator:IdGenerator) (getUtcDate: GetUtcDate) (published:PublishedChangeSet) =
        {
            Id = ChangeSetId (idGenerator())
            Parent = Some published.Id
            CreatedOn = getUtcDate()
        }
        
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]        
module PublishedChangeSet = 
    let create (idGenerator:IdGenerator) (getUtcDate: GetUtcDate) (toPublish:DraftChangeSet) =
        {
            Id = ChangeSetId (idGenerator())
            Parent = Some toPublish.Id
            CreatedOn = toPublish.CreatedOn
            PublishedOn = getUtcDate()
        }  
        
         
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ChangeSet =      

    let internal _createNewChangeSet 
        (idGenerator:IdGenerator) 
        (getUtcDate : GetUtcDate)
        (getCurrentChangeSet : StorageCurrentChangeSet)      
        (storeChangeSet : StoreChangeSet)   
        =
        asyncTrial {
            let! set = getCurrentChangeSet()
            match set with
            | ChangeSet.Draft draft ->
                return ChangeSet.Draft draft
            | ChangeSet.Published p ->
                return! asyncTrial {
                    let set = ChangeSet.Draft (DraftChangeSet.create idGenerator getUtcDate p)
                    return! storeChangeSet set
                }
        } 
    
    let createNewChangeSet = 
        _createNewChangeSet 
            (fun () -> Guid.NewGuid()) // TODO: review
            (fun () -> DateTime.UtcNow)// TODO: review
            (Ioc.resolve<StorageCurrentChangeSet>())  
            (Ioc.resolve<StoreChangeSet>())  
    
    
    let internal _draftToPublishedChangeSetContent (getUtcDate : GetUtcDate) publishToChangeset (content:DraftChangeSetContent) =
        let mapCategory (c: Category) = 
            match c with 
            | Category.Published p -> 
               p
            | Category.Draft draft -> 
                {
                    Id = draft.Id
                    Title  = draft.Title
                    Description = draft.Description
                    Slug  = draft.Slug
                    ParentId   = draft.ParentId
                    PublishedOn = getUtcDate()
                    ChangeSet = publishToChangeset 
                } 
         
        let mapFiles (s: File list) = 
             s |> List.map(
                    fun file -> 
                        match file with 
                        | File.Draft f ->
                           {
                                Id = f.Id
                                Name = f.Name
                                Slug = f.Slug
                                MimeType = f.MimeType
                                ChangeSet = publishToChangeset
                                PublishedOn = getUtcDate()
                            }
                        |  File.Published p -> p
                        )
        let mapDocuments (s : DraftDocument list) =
            s |> List.map(
                    fun d -> 
                    {
                        Id = d.Id
                        Title = d.Title
                        Content = d.Content
                        Category = (mapCategory d.Category.Value)
                        ChangeSet = publishToChangeset
                        Owner = d.Owner
                        Files = d.Files |> mapFiles
                        ExtraAttributes = d.ExtraAttributes
                        PublishedOn = getUtcDate()
                    })
                    
        {
            Files = content.Files |>  mapFiles
            Documents = content.Documents |> mapDocuments
            Categories  = content.Categories |> List.map mapCategory
        }
        
    let draftToPublishedChangeSetContent  = _draftToPublishedChangeSetContent Core.getUtcDate
        
    let internal _draftToPublishedChangeSet (utcGet: GetUtcDate) (changeSet:DraftChangeSet) =
        {
            Id = changeSet.Id
            Parent = changeSet.Parent
            CreatedOn = changeSet.CreatedOn
            PublishedOn = utcGet()
        }
    let private draftToPublishedChangeSet = _draftToPublishedChangeSet getUtcDate
                
    let internal _publish 
        (storageGetContent : StorageGetDraftChangeSetContent)
        (storagePublish : StoragePublishChangeSetContent)
        (draftToPublishedChangeSet: DraftChangeSet -> PublishedChangeSet)
        (draftToPublishedChangeSetContent: PublishedChangeSet -> DraftChangeSetContent -> PublishedChangeSetContent)
        (changeSet:DraftChangeSet)  
        =
        asyncTrial {
            let! content = (storageGetContent changeSet)  
            let publishedChangeSet = draftToPublishedChangeSet changeSet
            return! content
                |> draftToPublishedChangeSetContent publishedChangeSet
                |> storagePublish publishedChangeSet
        }
        
    let publish = 
        _publish 
            (Ioc.resolve<StorageGetDraftChangeSetContent>())
            (Ioc.resolve<StoragePublishChangeSetContent>())
            draftToPublishedChangeSet
            draftToPublishedChangeSetContent
            
            
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]            
module Document = 
   let _create (store: StorageStoreDraftDocument) (changeSet: DraftChangeSet) owner =
        {
            Id = DocumentId (Guid.NewGuid())
            Title  = Title "New Document"
            Content = ""
            Category = Option.None
            ChangeSet = changeSet
            Owner = owner
            Files = []
            ExtraAttributes = Map.empty
        }
        |> store           
        
   let create = _create (Ioc.resolve<StorageStoreDraftDocument>())
   
 
   let _getDraft
        (getChangeSet : StorageCurrentDraftChangeSet)
        (get: StorageGetDocumentCurrentDraft) 
        id 
        =
        asyncTrial {
            let! changeSet = getChangeSet()
            return! get changeSet id
        }
        
        
   let getDraft = 
        _getDraft 
            (Ioc.resolve<StorageCurrentDraftChangeSet>())
            (Ioc.resolve<StorageGetDocumentCurrentDraft>())
            
   
   let _createDraft          
        (store: StorageStoreDraftDocument) 
        (changeSet: DraftChangeSet) 
        (document : PublishedDocument)
        =
        asyncTrial {
            let draft = {
                Id = document.Id
                Title = document.Title
                Content = document.Content
                Owner = document.Owner   
                Category = Some (Category.Published document.Category)
                ChangeSet = changeSet 
                Files = document.Files |> List.map(File.Published)
                ExtraAttributes = document.ExtraAttributes    
            }
            
            return! store draft                 
        }
    
   let createDraft = _createDraft (Ioc.resolve<StorageStoreDraftDocument>())  