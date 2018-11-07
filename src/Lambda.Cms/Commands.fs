namespace Lambda.Cms

module CommandManager =
    open Chessie
    type DbUpdateTitle = DocumentId  -> Title -> Async<unit>
    
    type UpdateTitle = {
            DocumentId : DocumentId
            Title : Title
            UserId: UserId
            }
    
    type UpdateContent = {
            DocumentId : DocumentId
            Content : string
            UserId: UserId
            }
        
    type UpdateCategory = {
            DocumentId : DocumentId
            Category : CategoryId 
            UserId: UserId
            }
            
    type UpdateSlug = {
            DocumentId : DocumentId
            Slug : Slug 
            UserId: UserId
            }
              
    type CreateNewDraft = {
            DocumentId : DocumentId
            UserId: UserId
            }          
        
        
    let private failUpdateDocument = 
        fail<DraftDocument,RBad> (RBad.Message "Can't update document on a published changeset") 
                    |> Async.toAsyncResult        
        
        
    /// Create a new draft from an existing document    
    let createNewDraftCommand
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (storeDraftExists : StorageCheckDraftDocumentExists) 
        (getPublished: StorageGetPublishedDocument)
        (cmd: CreateNewDraft)
        =
        asyncTrial{
            let! cs = getCurrentChangeSet(cmd.UserId)
            match cs with 
            | Draft changeSet ->  
                let! exists = storeDraftExists changeSet cmd.DocumentId
                if (exists) then
                    return! Async.toAsyncResult (fail (RBad.Message "draft already exists"))
                else
                    let! published = getPublished cmd.DocumentId
                    return Document.createDraft changeSet.Id published
            | Published _ ->
                return! 
                    failUpdateDocument                      
        }            
            
    /// Update document title            
    let updateTitleCommand  
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (storeDraft : StorageStoreDraftDocument)
        (getDraft: StorageGetDocumentCurrentDraft)
        (update:UpdateTitle)
        userId
        =
        asyncTrial {
            let! cs = getCurrentChangeSet(userId)
            match cs with 
            | Draft changeSet ->            
                let! draft = getDraft changeSet update.DocumentId                             
                return! storeDraft {draft with Title = update.Title}       
            | Published _ ->
                return! 
                    failUpdateDocument                      
        }
            
    /// Update document content        
    let updateContentCommand 
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (getDraft: StorageGetDocumentCurrentDraft)   
        (storeDraft : StorageStoreDraftDocument)
        (e:UpdateContent)
        =
        asyncTrial {
            let! cs = getCurrentChangeSet(e.UserId)
            match cs with 
            | Draft changeSet ->         
                let! draft = getDraft changeSet e.DocumentId            
                return! storeDraft {draft with Content = e.Content}   
            | Published _ ->
                return! 
                    failUpdateDocument                        
        } 
            
    /// Update the document category        
    let updateCategoryCommand
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (getDraft: StorageGetDocumentCurrentDraft)         
        (storeDraft : StorageStoreDraftDocument)
        (getCategory: StorageGetCategory)
        (e:UpdateCategory)
        =
        asyncTrial {
            let! cs = getCurrentChangeSet(e.UserId)
            match cs with 
            | Draft changeSet -> 
                let! document = getDraft changeSet e.DocumentId
                let! category = getCategory e.Category (ChangeSet.Draft changeSet)            
                return! storeDraft {document with Category = (Some category)}           
            | Published _ ->
                return! 
                    failUpdateDocument
        }  
    
    /// Update the document Slug    
    let updateSlugCommand
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (getDraft: StorageGetDocumentCurrentDraft)         
        (storeDraft : StorageStoreDraftDocument)
        (getCategory: StorageGetCategory)
        (e:UpdateSlug)
        =
        asyncTrial {
            let! cs = getCurrentChangeSet(e.UserId)
            match cs with 
            | Draft changeSet -> 
                let! document = getDraft changeSet e.DocumentId         
                return! storeDraft {document with Slug = e.Slug}           
            | Published _ ->
                return! 
                    failUpdateDocument
        }          
    