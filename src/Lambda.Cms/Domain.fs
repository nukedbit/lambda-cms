namespace Lambda.Cms

open System

type ChangeSetDraftId = ChangeSetDraftId of Guid
    
type ChangeSetPublishedId = ChangeSetPublishedId of Guid

type ChangeSetId = 
    | Draft of ChangeSetDraftId
    | Published of ChangeSetPublishedId               

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ChangeSetDraftId  =
    let toPublishedId (ChangeSetDraftId id) =
        ChangeSetPublishedId id
        

type ChangeSetParentId = 
    | Root
    | PublishedId of ChangeSetPublishedId

type DraftFile = 
    {
        Id : FileId
        MimeType : string
        Name : string
        Slug : Slug      
    }
    
type PublishedFile = 
    {
        Id : FileId
        MimeType : string
        Name : string
        Slug : Slug
        PublishedOn: DateTime        
    } 
    
type File = 
    | Draft of DraftFile
    | Published of PublishedFile           

type User =
    {
        UserId : UserId   
        Email : Email
        Slug : Slug
    }

type DraftCategory = 
    {
        Id: CategoryId
        Title : Title
        Description: string
        Slug : Slug
        ParentId : CategoryId option
    }
    
type PublishedCategory = 
    {
        Id: CategoryId
        Title : Title
        Description: string
        Slug : Slug
        ParentId : CategoryId option
        PublishedOn: DateTime
    }
    
type Category = 
    | Draft of DraftCategory
    | Published of PublishedCategory    
    

type PublishedDocument = 
    {
        Id: DocumentId
        Title : Title
        Content : string
        Category : PublishedCategory
        Owner : UserId
        Files : PublishedFile list
        ExtraAttributes : Map<string,obj>
        PublishedOn: DateTime
        Slug: Slug
    }
    
type DraftDocument = 
    {
        Id: DocumentId
        Title : Title
        Content : string
        Category : Category option
        Owner : UserId
        Files : File list
        ExtraAttributes : Map<string,obj>
        Slug: Slug
    }    
    
type Document = 
    | Draft of DraftDocument
    | Published of PublishedDocument
    
    
    
type DraftChangeSet = {
    Id : ChangeSetDraftId
    Parent: ChangeSetParentId
    CreatedOn: DateTime
    Documents : DraftDocument list
    Categories: Category list
    Files: File list    
}

type PublishedChangeSet = {
    Id : ChangeSetPublishedId
    Parent: ChangeSetParentId
    CreatedOn: DateTime
    PublishedOn: DateTime
    Documents : PublishedDocument list
    Categories: PublishedCategory list
    Files: PublishedFile list       
}    
        
type ChangeSet = 
    | Draft of DraftChangeSet
    | Published of PublishedChangeSet

