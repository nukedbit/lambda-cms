namespace Lambda.Cms

module CommandManager =

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
    
    let processDocumentCommands 
        (updateTitle : DbUpdateTitle)
        (event:DocumentCommand) 
        =
        async {
            match event with 
            | ChangeTitleCommand e -> 
                do! updateTitle e.DocumentId e.Title
            | ChangeContentCommand e ->
                ()
            | ChangeCategoryCommand e ->
                ()
            | CreateNewDraftCommand e ->
                ()
            | NewDocumentCommand ->
                ()         
        }