namespace Lambda.Cms
open Chessie

type StorageCurrentChangeSet = unit -> AsyncResult<ChangeSet>

type StorageLastPublishedChangeSet = unit -> AsyncResult<PublishedChangeSet>   

type StoreChangeSet = ChangeSet -> AsyncResult<unit>

type StorageGetDraftChangeSetContent = DraftChangeSet -> AsyncResult<DraftChangeSetContent>

type StoragePublishChangeSetContent = PublishedChangeSet -> PublishedChangeSetContent -> AsyncResult<PublishedChangeSetContent>

