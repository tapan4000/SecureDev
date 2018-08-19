CREATE TABLE [dbo].[CityBasedPublicGroup]
(
	[CityBasedPublicGroupId] INT NOT NULL IDENTITY(1, 1),
	[CityId]					INT NOT NULL,
	[PublicGroupId]				INT NOT NULL,
	[CreatedBy]					VARCHAR(50) NOT NULL, 
    [CreationDateTime]			DATETIME2 NOT NULL, 
    [LastModifiedBy]			VARCHAR(50) NULL, 
    [LastModificationDateTime]	DATETIME2 NULL,
	CONSTRAINT [PK_dbo.CityBasedPublicGroup]	PRIMARY KEY CLUSTERED ([CityBasedPublicGroupId]),
	CONSTRAINT [UK_CityBasedPublicGroup]		UNIQUE ([CityId], [PublicGroupId])
)
