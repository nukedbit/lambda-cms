namespace Lambda.Cms

open Commands

module CommandsHandlers =
    open Chessie
    let private failUpdateDocument<'TSuccess> = 
        fail<'TSuccess,RBad> (RBad.Message "Can't update a published changeset") 
                    |> Async.toAsyncResult        
        
    let getCurrentDraftChangeSet
        (getCurrentChangeSet: StorageCurrentChangeSet)
        userId
        =
        asyncTrial{
            let! cs = getCurrentChangeSet(userId)
            match cs with 
            | Draft changeSet ->
                return changeSet
            | Published _ ->
                return! 
                    failUpdateDocument                 
        }            

        
    /// Create a new draft from an existing document    
    let handleCreateNewDraftCommand
        (storeDraftExists : StorageCheckDraftDocumentExists) 
        (getPublished: StorageGetPublishedDocument)
        (cmd: CreateNewDraftCommand)        
        =
        asyncTrial {
            let! exists = storeDraftExists cmd.ChangeSet cmd.DocumentId
            if (exists) then
                return! Async.toAsyncResult (fail (RBad.Message "draft already exists"))
            else
                let! published = getPublished cmd.DocumentId
                return Document.createDraft cmd.ChangeSet.Id published                    
        }    
            
    /// Update document title            
    let handleUpdateTitleCommand  
        (getDraft: StorageGetDocumentCurrentDraft)
        (cmd: UpdateTitleCommand)        
        =
        asyncTrial {           
            let! draft = getDraft cmd.ChangeSet cmd.DocumentId                             
            return {draft with Title = cmd.Title}                     
        }
            
    /// Update document content        
    let handleUpdateContentCommand
        (getDraft: StorageGetDocumentCurrentDraft)
        
        (cmd:UpdateContentCommand)
        =
        asyncTrial {        
            let! draft = getDraft cmd.ChangeSet cmd.DocumentId            
            return {draft with Content = cmd.Content}                      
        } 
            
    /// Update the document category        
    let handleUpdateCategoryCommand
        (getDraft: StorageGetDocumentCurrentDraft) 
        (getCategory: StorageGetCategory)
        (cmd:UpdateCategoryCommand)
        =
        asyncTrial {
            let! document = getDraft cmd.ChangeSet cmd.DocumentId
            let! category = getCategory cmd.Category (ChangeSet.Draft cmd.ChangeSet)            
            return {document with Category = (Some category)}
        }  
    
    /// Update the document Slug    
    let handleUpdateSlugCommand
        (getDraft: StorageGetDocumentCurrentDraft)
        (cmd:UpdateSlugCommand)
        =
        asyncTrial {
            let! document = getDraft cmd.ChangeSet cmd.DocumentId         
            return {document with Slug = cmd.Slug}           
        }          
    
    /// Update the document Slug    
    let handleUpdateExtraAttributesCommand
        (getDraft: StorageGetDocumentCurrentDraft)
        (cmd:UpdateExtraAttributesCommand)
        =
        asyncTrial {
            let! document = getDraft cmd.ChangeSet cmd.DocumentId         
            return {document with ExtraAttributes= cmd.Attributes}
        }
        
    /// Add a file to the document    
    let handleAddFileCommand
        (getDraft: StorageGetDocumentCurrentDraft)
        (cmd:AddDocumentFileCommand)
        =
        asyncTrial {
            let! document = getDraft cmd.ChangeSet cmd.DocumentId         
            return {document with Files = cmd.File :: document.Files}           
        }
        
    /// Add a file to the document    
    let handleRemoveFileCommand
        (getDraft: StorageGetDocumentCurrentDraft) 
        (cmd:RemoveDocumentFileCommand)
        =
        asyncTrial {
            let! document = getDraft cmd.ChangeSet cmd.DocumentId         
            return {document with Files = document.Files |> List.filter(fun f -> f = cmd.File) }
        }
       

