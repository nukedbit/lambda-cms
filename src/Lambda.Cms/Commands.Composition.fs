namespace Lambda.Cms

module CommandsComposed =
    open CommandsHandlers
        
    let handleCreateNewDraftCommand =
        handleCreateNewDraftCommand
            (Ioc.resolve<StorageCheckDraftDocumentExists>())
            (Ioc.resolve<StorageGetPublishedDocument>())
            
    let handleUpdateTitleCommand =
            handleUpdateTitleCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())
                
    let handleUpdateContentCommand =
            handleUpdateContentCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())
            
    let handleUpdateCategoryCommand =
            handleUpdateCategoryCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())
                (Ioc.resolve<StorageGetCategory>())
                
    let handleUpdateSlugCommand =
            handleUpdateSlugCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())
                
    let handleUpdateExtraAttributesCommand =
            handleUpdateExtraAttributesCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())
                
    let handleAddFileCommand =
            handleAddFileCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())
                
    let handleRemoveFileCommand =
            handleRemoveFileCommand
                (Ioc.resolve<StorageGetDocumentCurrentDraft>())                    

    
