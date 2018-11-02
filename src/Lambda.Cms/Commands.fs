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
        
        
    let internal _createNewDraftCommand
        (currentDraftChangeSet: StorageCurrentDraftChangeSet)
        (storeDraftExists : StorageCheckDraftDocumentExists) 
        (getPublished: StorageGetPublishedDocument)
        (cmd: NewDraft)
        =
        asyncTrial{
            let! changeset = currentDraftChangeSet()
            let! exists = storeDraftExists changeset cmd.DocumentId
            if (exists) then
                return! Async.toAsyncResult ( fail (RBad.Message "draft already exists"))
            else
                let! published = getPublished cmd.DocumentId
                return! Document.createDraft changeset published
        }
        
    
    let createNewDraftCommand =
        _createNewDraftCommand 
            (Ioc.resolve<StorageCurrentDraftChangeSet>())
            (Ioc.resolve<StorageCheckDraftDocumentExists>())
            (Ioc.resolve<StorageGetPublishedDocument>())
            
            
            
    let _updateTitleCommand  
        (storeDraft : StorageStoreDraftDocument)
        (getDraft : DocumentId -> AsyncResult<DraftDocument, RBad>)
        (update:UpdateTitle)
        =
        asyncTrial {
            let! draft = getDraft update.DocumentId                 
            {draft with Title = update.Title}
            return! storeDraft draft            
        }             
       
    let updateTitleCommand =
        _updateTitleCommand 
            (Ioc.resolve<StorageStoreDraftDocument>())
            Document.getDraft
            
            
    let _updateContentCommand 
        (getDraft : DocumentId -> AsyncResult<DraftDocument, RBad>)
        (storeDraft : StorageStoreDraftDocument)
        (e:UpdateContent)
        =
        asyncTrial {
            let! draft = getDraft e.DocumentId
            {draft with Content = e.Content}
            return! storeDraft draft           
        } 
        

    let updateContentCommand = 
        _updateContentCommand
            Document.getDraft
            (Ioc.resolve<StorageStoreDraftDocument>())
            
            
    let _updateCategoryCommand 
        (getDraft : DocumentId -> AsyncResult<DraftDocument, RBad>)
        (storeDraft : StorageStoreDraftDocument)
        (getCategory: StorageGetCategory)
        (e:UpdateCategory)
        =
        asyncTrial {
            let! draft = getDraft e.DocumentId
            let! category = getCategory e.Category (ChangeSet.Draft draft.ChangeSet)
            {draft with Category = (Some category)}
            return! storeDraft draft           
        }                                             
        
    let updateCategoryCommand = 
        _updateCategoryCommand
            Document.getDraft
            (Ioc.resolve<StorageStoreDraftDocument>())    
            (Ioc.resolve<StorageGetCategory>())    
    