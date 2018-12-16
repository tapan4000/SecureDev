CREATE TABLE [dbo].[Application]
(
	[ApplicationId]								INT NOT NULL,
	[ApplicationUniqueId]						VARCHAR(50) NOT NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL,
	CONSTRAINT [PK_dbo.Application]				PRIMARY KEY CLUSTERED ([ApplicationId]),
	CONSTRAINT [UK_dbo.ApplicationUniqueId]		UNIQUE ([ApplicationUniqueId])
)
