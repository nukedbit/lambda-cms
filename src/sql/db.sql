create database lambdacms;

use lambdacms;

GO
create table dbo.Category  (
  CategoryId UNIQUEIDENTIFIER NOT NULL ,
    CONSTRAINT [PK_Category] PRIMARY KEY  CLUSTERED (CategoryId),
  Title varchar(100),
  Slug varchar(100) UNIQUE ,
  ParentID UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_Category_Parent] FOREIGN KEY  (CategoryId)
    REFERENCES dbo.Category(CategoryId),
  [Path] HIERARCHYID NULL 
);

GO
create table dbo.[User] (
  UserId UNIQUEIDENTIFIER NOT NULL,
  CONSTRAINT [PK_User] PRIMARY KEY  CLUSTERED (UserId),
  Email varchar(254),
  Slug varchar(100) UNIQUE,
  Attributes varchar(max) NULL 
    CHECK(ISJSON(Attributes) > 0)
);

GO

create table dbo.[Media] (
  MediaId UNIQUEIDENTIFIER NOT NULL,
  CONSTRAINT [PK_Media] PRIMARY KEY  CLUSTERED (MediaId),
  MimeType varchar(20)  NOT NULL,
  Name varchar(100) NOT NULL,
  Slug varchar(100) NOT NULL,
  DocumentId UNIQUEIDENTIFIER NOT NULL,
  Attributes varchar(max) NULL
    CHECK(ISJSON(Attributes) > 0)  
);

GO

create table dbo.[Document] (
  DocumentId UNIQUEIDENTIFIER NOT NULL,
  CONSTRAINT [PK_Document] PRIMARY KEY  CLUSTERED (DocumentId),
  Title varchar(100),
  Content varchar(MAX),
  CategoryId UNIQUEIDENTIFIER  NOT NULL,
  CONSTRAINT [FK_CategoryId] FOREIGN KEY  (CategoryId)
    REFERENCES dbo.Category(CategoryId),
  Version INT NOT NULL DEFAULT 0,
  Status varchar(50),
  DefaultMediaId UNIQUEIDENTIFIER NULL,
  CONSTRAINT [FK_MediaId] FOREIGN KEY  (DefaultMediaId)
    REFERENCES dbo.[Media](MediaId),
  Owner UNIQUEIDENTIFIER  NOT NULL,
  CONSTRAINT [FK_UserId] FOREIGN KEY  (Owner)
    REFERENCES dbo.[User](UserId),
  Attributes varchar(max) NULL
    CHECK(ISJSON(Attributes) > 0)
);

GO
ALTER TABLE dbo.[Media]
  ADD CONSTRAINT [FK_DocumentId] FOREIGN KEY  (DocumentId)
  REFERENCES dbo.[Document](DocumentId)
GO