namespace  Lambda.Cms

module CommandsManager =

    open Commands
    
    module Document =        
        open Commands.Document
        open CommandsComposition.Document
                
        let handleDocumentCommands (c:IDocumentCommand) =
            match c with
            | :? CreateNewDraftCommand as cmd ->
                handleCreateNewDraftCommand cmd
            | :? UpdateTitleCommand as cmd ->
                handleUpdateTitleCommand cmd
            | :? UpdateContentCommand as cmd ->
                handleUpdateContentCommand cmd
            | :? UpdateCategoryCommand as cmd ->
                handleUpdateCategoryCommand cmd
            | :? UpdateSlugCommand as cmd ->
                handleUpdateSlugCommand cmd
            | :? UpdateExtraAttributesCommand as cmd ->
                handleUpdateExtraAttributesCommand cmd
            | :? AddDocumentFileCommand as cmd ->
                handleAddFileCommand cmd
            | :? RemoveDocumentFileCommand as cmd ->
                handleRemoveFileCommand cmd
            | _ ->
                Trial.fail (RBad.Message "unhandled command") |> Async.toAsyncResult    