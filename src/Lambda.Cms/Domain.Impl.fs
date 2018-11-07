namespace Lambda.Cms

open System
open Chessie
open Core

module DomaingMapping = 

    let mapCategory (utcNow:DateTime) (c: Category) = 
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
                PublishedOn = utcNow
            } 
     
    let mapFiles  (utcNow:DateTime) (s: File list) = 
         s |> List.map(
                fun file -> 
                    match file with 
                    | File.Draft f ->
                       {
                            Id = f.Id
                            Name = f.Name
                            Slug = f.Slug
                            MimeType = f.MimeType
                            PublishedOn = utcNow
                        }
                    |  File.Published p -> p
                    )
                    
    let mapDocuments  (utcNow:DateTime) (s : DraftDocument list) =
        s |> List.map(
                fun d -> 
                {
                    Id = d.Id
                    Title = d.Title
                    Content = d.Content
                    Category = (mapCategory utcNow d.Category.Value)
                    Owner = d.Owner
                    Files = d.Files |> mapFiles utcNow
                    ExtraAttributes = d.ExtraAttributes
                    PublishedOn = getUtcDate()
                })

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module DraftChangeSet = 
    let create (idGenerator:IdGenerator) (utcNow:DateTime) (published:PublishedChangeSet) =
        {
            Id = ChangeSetDraftId (idGenerator())
            Parent = ChangeSetParentId.PublishedId published.Id
            CreatedOn = utcNow
            Files = []
            Documents = []
            Categories = []
        }
        
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]        
module PublishedChangeSet = 
    let create (idGenerator:IdGenerator) (utcNow:DateTime) (toPublish:DraftChangeSet) =
        {
            Id = ChangeSetDraftId.toPublishedId toPublish.Id
            Parent = toPublish.Parent
            CreatedOn = toPublish.CreatedOn
            PublishedOn = utcNow
            Files = toPublish.Files |> DomaingMapping.mapFiles utcNow
            Documents = toPublish.Documents |> DomaingMapping.mapDocuments utcNow
            Categories = toPublish.Categories |> List.map(DomaingMapping.mapCategory utcNow)
        }  
        
         
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ChangeSet =      

    let createNewChangeSet 
        (idGenerator:IdGenerator) 
        (utcNowDateTime : DateTime)    
        (currentChangeSet: ChangeSet)
        : Chessie.Result<ChangeSet, RBad>
        =
        trial {
            match currentChangeSet with
            | ChangeSet.Draft (cs) ->
                return ChangeSet.Draft(cs)
            | ChangeSet.Published (cs) ->
                let newChangeSet = (DraftChangeSet.create idGenerator utcNowDateTime cs)
                return ChangeSet.Draft (newChangeSet)
        } 
        
              
    let publish 
        (idGenerator:IdGenerator) 
        (utcNowDateTime : DateTime) 
        (changeSet:ChangeSet)  
        =
        trial {
            match changeSet with 
            | Published _ ->
                return! fail<PublishedChangeSet,RBad> (RBad.Message "")
            | Draft (dcs) ->
                return PublishedChangeSet.create idGenerator utcNowDateTime dcs
        }
            
            
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]            
module Document = 
   let create (changeSet: DraftChangeSet) owner =
        {
            Id = DocumentId (Guid.NewGuid())
            Title  = Title "New Document"
            Content = ""
            Category = Option.None
            Owner = owner
            Files = []
            ExtraAttributes = Map.empty
        }
   
 
   let getDraft
        (getChangeSet : StorageCurrentDraftChangeSet)
        (get: StorageGetDocumentCurrentDraft) 
        id 
        =
        asyncTrial {
            let! changeSet = getChangeSet()
            return! get changeSet id
        }
            
   
   let createDraft          
        (changeSetId: ChangeSetDraftId) 
        (document : PublishedDocument)
        =
        {
            Id = document.Id
            Title = document.Title
            Content = document.Content
            Owner = document.Owner   
            Category = Some (Category.Published document.Category)
            Files = document.Files |> List.map(File.Published)
            ExtraAttributes = document.ExtraAttributes    
        }