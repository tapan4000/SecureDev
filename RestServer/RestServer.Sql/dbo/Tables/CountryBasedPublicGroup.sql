CREATE TABLE [dbo].[CountryBasedPublicGroup]
(
	[CountryBasedPublicGroupId] INT NOT NULL IDENTITY(1, 1),
	[CountryId]					INT NOT NULL,
	[PublicGroupId]				INT NOT NULL,
	[CreatedBy]					VARCHAR(50) NOT NULL, 
    [CreationDateTime]			DATETIME2 NOT NULL, 
    [LastModifiedBy]			VARCHAR(50) NULL, 
    [LastModificationDateTime]	DATETIME2 NULL,
	CONSTRAINT [PK_dbo.CountryBasedPublicGroup]	PRIMARY KEY CLUSTERED ([CountryBasedPublicGroupId]),
	CONSTRAINT [UK_CountryBasedPublicGroup]		UNIQUE ([CountryId], [PublicGroupId])
)
