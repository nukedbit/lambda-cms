namespace Lambda.Cms

module CommandManager =
    open Chessie
    type DbUpdateTitle = DocumentId  -> Title -> Async<unit>
    
    type UpdateTitle = {
            DocumentId : DocumentId
            Title : Title
        }
    
    type UpdateContent = {
            DocumentId : DocumentId
            Content : string
        }
        
    type UpdateCategory = {
            DocumentId : DocumentId
            Category : CategoryId            
        }
    
    type NewDraft = {
            DocumentId : DocumentId
        }

    type DocumentCommand =
        | ChangeTitleCommand of UpdateTitle
        | ChangeContentCommand of UpdateContent
        | ChangeCategoryCommand of UpdateCategory
        | CreateNewDraftCommand of NewDraft
        | NewDocumentCommand 
        
        
    let private failUpdateDocument = 
        fail<DraftDocument,RBad> (RBad.Message "Can't update document on a published changeset") 
                    |> Async.toAsyncResult        
        
    let createNewDraftCommand
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (storeDraftExists : StorageCheckDraftDocumentExists) 
        (getPublished: StorageGetPublishedDocument)
        (cmd: NewDraft)
        =
        asyncTrial{
            let! cs = getCurrentChangeSet()
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
            
    let updateTitleCommand  
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (storeDraft : StorageStoreDraftDocument)
        (getDraft: StorageGetDocumentCurrentDraft)
        (update:UpdateTitle)
        =
        asyncTrial {
            let! cs = getCurrentChangeSet()
            match cs with 
            | Draft changeSet ->            
                let! draft = getDraft changeSet update.DocumentId                             
                return! storeDraft {draft with Title = update.Title}       
            | Published _ ->
                return! 
                    failUpdateDocument                      
        }
            
            
    let updateContentCommand 
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (getDraft: StorageGetDocumentCurrentDraft)   
        (storeDraft : StorageStoreDraftDocument)
        (e:UpdateContent)
        =
        asyncTrial {
            let! cs = getCurrentChangeSet()
            match cs with 
            | Draft changeSet ->         
                let! draft = getDraft changeSet e.DocumentId            
                return! storeDraft {draft with Content = e.Content}   
            | Published _ ->
                return! 
                    failUpdateDocument                        
        } 
            
            
    let updateCategoryCommand
        (getCurrentChangeSet: StorageCurrentChangeSet)
        (getDraft: StorageGetDocumentCurrentDraft)         
        (storeDraft : StorageStoreDraftDocument)
        (getCategory: StorageGetCategory)
        (e:UpdateCategory)
        =
        asyncTrial {
            let! cs = getCurrentChangeSet()
            match cs with 
            | Draft changeSet -> 
                let! document = getDraft changeSet e.DocumentId
                let! category = getCategory e.Category (ChangeSet.Draft changeSet)            
                return! storeDraft {document with Category = (Some category)}           
            | Published _ ->
                return! 
                    failUpdateDocument
        }  
    