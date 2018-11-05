namespace Lambda.Cms.Tests

module Tests =
    open System
    open Xunit
    open FsUnit.Xunit 
    open Lambda.Cms
    open Lambda.Cms
    
    
    [<Fact>]
    let ``Create New ChangeSet when already has one in draft return it`` () =
        let cid = Guid.NewGuid()
        let getCurrentUtcNow = fun() -> new DateTime(1900,1,1)
        let idGenerator = fun () -> cid
        let changeSet = {
                DraftChangeSet.Id = ChangeSetId cid
                Parent = Option.None
                CreatedOn = DateTime(1900,1,1)
            }
        let currentChangeSetStoreMock = 
             (fun () -> 
                let c = ChangeSet.Draft changeSet
                let r = Chessie.Result<ChangeSet,RBad>.Ok(c, [])
                Async.toAsyncResult(r)   
             )
        let storeMock = fun (cset) -> Chessie.Result<ChangeSet,RBad>.Succeed(cset) |> Async.toAsyncResult
                    
        let result = 
            ChangeSet._createNewChangeSet idGenerator getCurrentUtcNow currentChangeSetStoreMock storeMock 
            |> Async.ofAsyncResult
            |> Async.RunSynchronously
            
        result |> should equal (Chessie.Result<ChangeSet,RBad>.Ok(ChangeSet.Draft changeSet, []))
        
    [<Fact>]
    let ``Create New ChangeSet when current is marked as published`` () =
        let cid = Guid.NewGuid()
        let getCurrentUtcNow = fun() -> new DateTime(1900,10,10)
        let idGenerator = fun () -> Guid.Parse("743c3d2c-2e55-4cde-8dce-3997980aad77")
        let changeSet = {
                PublishedChangeSet.Id = ChangeSetId cid
                Parent = Option.None
                CreatedOn = DateTime(1900,1,1)
                PublishedOn = DateTime(1900,1,1)
            }
        let expected = {
                DraftChangeSet.Id = ChangeSetId (idGenerator())
                Parent = Some changeSet.Id
                CreatedOn = DateTime(1900,10,10)
            }            
        let currentChangeSetStoreMock = 
             (fun () -> 
                let c = ChangeSet.Published changeSet
                let r = Chessie.Result<ChangeSet,RBad>.Ok(c, [])
                Async.toAsyncResult(r)   
             )
        let storeMock = fun (cset) -> Chessie.Result<ChangeSet,RBad>.Succeed(cset) |> Async.toAsyncResult
                    
        let result = 
            ChangeSet._createNewChangeSet idGenerator getCurrentUtcNow currentChangeSetStoreMock storeMock 
            |> Async.ofAsyncResult
            |> Async.RunSynchronously
         
        result |> should equal (Chessie.Result<ChangeSet,RBad>.Ok(ChangeSet.Draft expected, []))
                 
