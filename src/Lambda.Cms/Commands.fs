namespace Lambda.Cms

module Commands =
    type DbUpdateTitle = DocumentId  -> Title -> Async<unit>
    
    type ICommand =
        interface
            abstract member UserId : UserId with get
        end

    type IDocumentCommand =
         inherit ICommand
    
    type UpdateTitleCommand =
        {
            DocumentId : DocumentId
            Title : Title
            UserId: UserId
            ChangeSet: DraftChangeSet
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId 
    
    type UpdateContentCommand =
        {
            DocumentId : DocumentId
            Content : string
            UserId: UserId
            ChangeSet: DraftChangeSet
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId         
        
    type UpdateCategoryCommand =
        {
            DocumentId : DocumentId
            Category : CategoryId 
            UserId: UserId
            ChangeSet: DraftChangeSet
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId          
            
    type UpdateSlugCommand =
        {
            DocumentId : DocumentId
            Slug : Slug 
            UserId: UserId
            ChangeSet: DraftChangeSet
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId          
              
    type CreateNewDraftCommand =
        {
            DocumentId : DocumentId
            UserId: UserId
            ChangeSet: DraftChangeSet
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId  
    
    
    type UpdateExtraAttributesCommand =
        {
            DocumentId : DocumentId
            UserId: UserId
            Attributes: Map<string, obj>
            ChangeSet: DraftChangeSet                                    
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId  
    
    type AddDocumentFileCommand =
        {
            DocumentId : DocumentId
            UserId: UserId
            File: File
            ChangeSet: DraftChangeSet                               
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId          
    
    type RemoveDocumentFileCommand =
        {
            DocumentId : DocumentId
            UserId: UserId
            File: File
            ChangeSet: DraftChangeSet                                     
        }
        interface IDocumentCommand with
            member x.UserId = x.UserId        
        
 