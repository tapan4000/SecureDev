CREATE TABLE [dbo].[LocalityBasedPublicGroup]
(
	[LocalityBasedPublicGroupId] INT NOT NULL IDENTITY(1, 1),
	[LocalityId]					INT NOT NULL,
	[PublicGroupId]				INT NOT NULL,
	[CreatedBy]					VARCHAR(50) NOT NULL, 
    [CreationDateTime]			DATETIME2 NOT NULL, 
    [LastModifiedBy]			VARCHAR(50) NULL, 
    [LastModificationDateTime]	DATETIME2 NULL,
	CONSTRAINT [PK_dbo.LocalityBasedPublicGroup]	PRIMARY KEY CLUSTERED ([LocalityBasedPublicGroupId]),
	CONSTRAINT [UK_LocalityBasedPublicGroup]		UNIQUE ([LocalityId], [PublicGroupId])
)
