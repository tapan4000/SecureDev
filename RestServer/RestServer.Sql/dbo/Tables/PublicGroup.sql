CREATE TABLE [dbo].[PublicGroup]
(
	[PublicGroupId]						INT NOT NULL IDENTITY (1, 1), 
    [GroupId]									INT NOT NULL, 
    [IsVerified]								BIT NOT NULL, 
    [VerifiedTitle]								VARCHAR(50) NULL, 
    [VerifiedDescription]						VARCHAR(50) NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL,
	CONSTRAINT [PK_dbo.PublicGroupMetadata]		PRIMARY KEY CLUSTERED ([PublicGroupId])
)
