open Lambda.Cms

namespace Lambda.Cms
open System
open Chessie

module DraftChangeSet = 
    let create (published:PublishedChangeSet) =
        {
            Id = ChangeSetId (Guid.NewGuid())
            Parent = published.Id
            CreatedOn = DateTime.UtcNow
        }
        
module PublishedChangeSet = 
    let create (published:DraftChangeSet) =
        {
            Id = ChangeSetId (Guid.NewGuid())
            Parent = published.Id
            CreatedOn = published.CreatedOn
            PublishedOn = DateTime.UtcNow
        }  
        
         

module ChangeSet =      
    open Chessie
    let _createNewChangeSet 
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
                    let set = ChangeSet.Draft (DraftChangeSet.create p)
                    do! storeChangeSet set
                    return set
                }
        } 
    
    let createNewChangeSet = _createNewChangeSet (Ioc.resolve<StorageCurrentChangeSet>())  
    
    
    let draftToPublishedChangeSetContent publishToChangeset (content:DraftChangeSetContent) =
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
                    PublishedOn = DateTime.UtcNow
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
                                PublishedOn = DateTime.UtcNow
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
                        PublishedOn = DateTime.UtcNow
                    })
                    
        {
            Files = content.Files |> List.map(File.Draft) |>  mapFiles
            Documents = content.Documents |> mapDocuments
            Categories  = content.Categories |> List.map(Category.Draft) |> List.map mapCategory
        }
        
    let draftToPublishedChangeSet (changeSet:DraftChangeSet) =
        {
            Id = changeSet.Id
            Parent = changeSet.Parent
            CreatedOn = changeSet.CreatedOn
            PublishedOn = DateTime.UtcNow
        }
                
    let _publish 
        (storageGetContent : StorageGetDraftChangeSetContent)
        (storagePublish : StoragePublishChangeSetContent)
        (changeSet:DraftChangeSet)  
        =
        asyncTrial {
            let! content = (storageGetContent changeSet)  
            let publishedChangeSet = draftToPublishedChangeSet changeSet
            return content 
                |> draftToPublishedChangeSetContent publishedChangeSet
                |> storagePublish publishedChangeSet
        }  
        
    let publish = 
        _publish 
            (Ioc.resolve<StorageGetDraftChangeSetContent>())
            (Ioc.resolve<StoragePublishChangeSetContent>())