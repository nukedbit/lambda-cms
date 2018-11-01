namespace Lambda.Cms

open System

type ChangeSetId = ChangeSetId of Guid


type DraftChangeSet = {
    Id : ChangeSetId
    Parent: ChangeSetId
    CreatedOn: DateTime
}

type PublishedChangeSet = {
    Id : ChangeSetId
    Parent: ChangeSetId
    CreatedOn: DateTime
    PublishedOn: DateTime
}

type ChangeSet = 
    | Draft of DraftChangeSet
    | Published of PublishedChangeSet

type DraftFile = 
    {
        Id : FileId
        MimeType : string
        Name : string
        Slug : Slug
        ChangeSet: DraftChangeSet        
    }
    
type PublishedFile = 
    {
        Id : FileId
        MimeType : string
        Name : string
        Slug : Slug
        ChangeSet: PublishedChangeSet
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
        ChangeSet: DraftChangeSet
    }
    
type PublishedCategory = 
    {
        Id: CategoryId
        Title : Title
        Description: string
        Slug : Slug
        ParentId : CategoryId option
        PublishedOn: DateTime
        ChangeSet: PublishedChangeSet
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
        ChangeSet : PublishedChangeSet
        Owner : UserId
        Files : PublishedFile list
        ExtraAttributes : Map<string,obj>
        PublishedOn: DateTime
    }
    
type DraftDocument = 
    {
        Id: DocumentId
        Title : Title
        Content : string
        Category : Category option
        ChangeSet : DraftChangeSet
        Owner : UserId
        Files : File list
        ExtraAttributes : Map<string,obj>
    }    
    
type Document = 
    | Draft of DraftDocument
    | Published of PublishedDocument
    
    
type DraftChangeSetContent = {
        Documents : DraftDocument list
        Categories: DraftCategory list
        Files: DraftFile list
    }   

type PublishedChangeSetContent = {
        Documents : PublishedDocument list
        Categories: PublishedCategory list
        Files: PublishedFile list        
    }   
     
type ChangeSetContent =
    | Draft of DraftChangeSetContent    
    | Published of PublishedChangeSetContent
        


