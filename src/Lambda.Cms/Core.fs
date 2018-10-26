namespace Lambda.Cms 

module Core =
    open Domain

    type Filter =
        | CategoryFilter of SlugType.Slug
        | IncludeStatus of DocumentStatus
        | ExcludeStatus of DocumentStatus

    type Page = 
        { 
            Index : int
            Size : int
        }

    type DbGetDocumentsAsync = (Page * list<Filter>) -> Async<Document list>
    type DbGetDocumentAsync = SlugType.Slug -> Async<Document>