CREATE TABLE [dbo].[StateBasedPublicGroup]
(
	[StateBasedPublicGroupId] INT NOT NULL IDENTITY(1, 1),
	[StateId]					INT NOT NULL,
	[PublicGroupId]				INT NOT NULL,
	[CreatedBy]					VARCHAR(50) NOT NULL, 
    [CreationDateTime]			DATETIME2 NOT NULL, 
    [LastModifiedBy]			VARCHAR(50) NULL, 
    [LastModificationDateTime]	DATETIME2 NULL,
	CONSTRAINT [PK_dbo.StateBasedPublicGroup]	PRIMARY KEY CLUSTERED ([StateBasedPublicGroupId]),
	CONSTRAINT [UK_StateBasedPublicGroup]		UNIQUE ([StateId], [PublicGroupId])
)
