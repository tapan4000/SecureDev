CREATE TABLE [dbo].[PublicGroup]
(
	-- A public group should always show the verified title and verified description as searching will be based on verified title, the table should be small, so a dedicated table
	-- for public group is needed.
    [GroupId]									INT NOT NULL, 
    [IsVerified]								BIT NOT NULL, 
	[VerifiedGroupCategoryId]					INT NOT NULL,
    [VerifiedTitle]								VARCHAR(50) NULL, 
    [VerifiedDescription]						VARCHAR(50) NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL,
	CONSTRAINT [PK_dbo.PublicGroupMetadata]		PRIMARY KEY CLUSTERED ([GroupId]),
	CONSTRAINT [UK_PublicGroupVerifiedTitle]	UNIQUE ([VerifiedTitle])
)
