namespace Lambda.Cms

module Services =
    open Domain
    open Ioc
    open Core    

    let getDocumentsAsync (page: Page, filters: Filter list) =
        let dbGet = Ioc.resolve<DbGetDocumentsAsync>()
        dbGet(page, filters)

    let getDocumentAsync (slug : SlugType.Slug) = 
        let dbGet = Ioc.resolve<DbGetDocumentAsync>()
        dbGet(slug)
