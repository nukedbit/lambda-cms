namespace Lambda.Cms

open Chessie

type StorageCurrentChangeSet = UserId -> AsyncResult<ChangeSet, RBad>

type StorageLastPublishedChangeSet = UserId -> AsyncResult<PublishedChangeSet, RBad>

type StoreChangeSet = ChangeSet -> AsyncResult<ChangeSet, RBad>

//type StorageGetDraftChangeSetContent = DraftChangeSet -> AsyncResult<DraftChangeSetContent, RBad>
    
type StorageGetChangeSet = ChangeSetId -> AsyncResult<ChangeSet, RBad>

type StoragePublishChangeSetContent = PublishedChangeSet -> AsyncResult<PublishedChangeSet, RBad>

type StorageStoreDraftDocument = DraftDocument -> AsyncResult<DraftDocument, RBad>

type StorageCurrentDraftChangeSet = UserId -> AsyncResult<DraftChangeSet, RBad>

type StorageGetDocumentCurrentDraft = DraftChangeSet -> DocumentId -> AsyncResult<DraftDocument, RBad>

type StorageGetCategory = CategoryId -> ChangeSet -> AsyncResult<Category, RBad>

type StorageCheckDraftDocumentExists = DraftChangeSet -> DocumentId -> Async<bool>

type StorageGetPublishedDocument = DocumentId -> AsyncResult<PublishedDocument, RBad>