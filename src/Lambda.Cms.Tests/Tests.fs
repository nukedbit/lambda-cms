namespace Lambda.Cms.Tests

module Tests =
    open System
    open Xunit
    open FsUnit.Xunit 
    open Lambda.Cms
    open Lambda.Cms
    
    
    [<Fact>]
    let ``Create New ChangeSet when already current is draft return it`` () =
        let cid = Guid.NewGuid()
        let getCurrentUtcNow = DateTime(1900,1,1)
        let idGenerator = fun () -> cid
        let changeSet = {
                Id = ChangeSetDraftId cid
                Parent = ChangeSetParentId.Root
                CreatedOn = getCurrentUtcNow
                Files = []
                Documents = []
                Categories = []
            }
                    
        let result = 
            ChangeSet.createNewChangeSet idGenerator getCurrentUtcNow (ChangeSet.Draft changeSet)            
            
        result |> should equal (Chessie.Result<ChangeSet,RBad>.Ok(ChangeSet.Draft changeSet, []))
        
    [<Fact>]
    let ``Create New ChangeSet when current is published`` () =
        let cid = Guid.NewGuid()
        let getCurrentUtcNow = new DateTime(1900,10,10)
        let idGenerator = fun () -> Guid.Parse("743c3d2c-2e55-4cde-8dce-3997980aad77")
        let changeSet = {
                Id = ChangeSetPublishedId cid
                Parent = ChangeSetParentId.Root
                CreatedOn = DateTime(1900,1,1)
                PublishedOn = DateTime(1900,1,1)
                Files = []
                Documents = []
                Categories = []
            }
            
        let expected = {
                Id = ChangeSetDraftId (idGenerator())
                Parent = ChangeSetParentId.PublishedId changeSet.Id
                CreatedOn = DateTime(1900,10,10)
                Files = []
                Documents = []
                Categories = []
            }
             
        let result = 
            ChangeSet.createNewChangeSet idGenerator getCurrentUtcNow (ChangeSet.Published changeSet)

         
        result |> should equal (Chessie.Result<ChangeSet,RBad>.Ok(ChangeSet.Draft expected, []))
                 

    [<Fact>]
    let ``Publish ChangeSet`` () =
        // SETUP
        let cid = Guid.NewGuid()
        let idGenerator = fun () -> cid
        let categoryId  =  Guid.NewGuid()
        let changeSet = {
            DraftChangeSet.Id = ChangeSetDraftId cid
            Parent = ChangeSetParentId.Root
            CreatedOn = DateTime(1900,1,1)
            Files = []
            Documents = []
            Categories = [
                Category.Draft {
                    Id = CategoryId categoryId
                    Title = Title "Test"
                    Description  = ""
                    Slug = Slug "slug"
                    ParentId = None
                }
            ]            
        }
        let utcNow = new DateTime(1900,10,10)
             
        let expectedPublishedChangeSet = {
            Id = ChangeSetPublishedId cid
            Parent = ChangeSetParentId.Root
            CreatedOn = DateTime(1900,1,1)
            PublishedOn = utcNow
            Files = []
            Documents = [] 
            Categories = [
                    {
                        Id = CategoryId categoryId
                        Title = Title "Test"
                        Description  = ""
                        Slug = Slug "slug"
                        ParentId = None
                        PublishedOn = utcNow
                    }
                ]
            }
        
        // ACT
        let publishFunc = 
            ChangeSet.publish 
                idGenerator 
                utcNow
                
        let result = 
            publishFunc (ChangeSet.Draft changeSet)
            
        // ASSERT
        match result with 
        | Ok (r, _) ->
            r |> should equal expectedPublishedChangeSet
        | Bad r ->
            failwithf "Failed result %s" ( String.Join(" ", r.ToString()))
        
        
    [<Theory>]
    [<InlineData("F# is the most beautiful Programming Language", "f-is-the-most-beautiful-programming-language")>]
    [<InlineData("È un buon giorno per morire!", "e-un-buon-giorno-per-morire")>]
    let ``Slug From String`` (str: string, expected: string) =
        let slug = (Slug.fromString str)
        match slug with
        | Ok (s, _) ->
            s |> Slug.value |> should equal expected
        | Bad e ->
            failwith (e |> List.head)     
        ()