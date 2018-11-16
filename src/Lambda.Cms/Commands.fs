namespace Lambda.Cms

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Commands =
    type DbUpdateTitle = DocumentId  -> Title -> Async<unit>
    
    type ICommand =
        interface
            abstract member UserId : UserId with get
        end
            
    type ICategoryCommand =
         inherit ICommand

    type IDocumentCommand =
         inherit ICommand
         
    module Document =         
    
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
            
    
    module Category =
        type NewCategoryCommand =
            {
                 UserId: UserId
                 ChangeSet: DraftChangeSet                 
            }
            interface ICategoryCommand with
                member x.UserId = x.UserId
                
        type UpdateCategoryTitlecommand =
            {
                 UserId: UserId
                 ChangeSet: DraftChangeSet
                 Title: Title              
            }
            interface ICategoryCommand with
                member x.UserId = x.UserId

        type UpdateCategorySlugcommand =
            {
                 UserId: UserId
                 ChangeSet: DraftChangeSet
                 Slug: Slug              
            }
            interface ICategoryCommand with
                member x.UserId = x.UserId
                
        type UpdateCategoryDescriptioncommand =
            {
                 UserId: UserId
                 ChangeSet: DraftChangeSet
                 Description: string              
            }
            interface ICategoryCommand with
                member x.UserId = x.UserId
                  
        type UpdateCategoryParentcommand =
            {
                 UserId: UserId
                 ChangeSet: DraftChangeSet
                 Parent: CategoryId option              
            }
            interface ICategoryCommand with
                member x.UserId = x.UserId                                                           