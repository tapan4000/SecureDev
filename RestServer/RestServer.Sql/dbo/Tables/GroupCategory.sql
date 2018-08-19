CREATE TABLE [dbo].[GroupCategory]
(
	[GroupCategoryId]					INT NOT NULL, 
    [CategoryName]						VARCHAR(50) NOT NULL, 
    [CategoryDescription]				VARCHAR(500) NULL,
	[CreatedBy]							VARCHAR(50) NOT NULL, 
    [CreationDateTime]					DATETIME2 NOT NULL, 
    [LastModifiedBy]					VARCHAR(50) NULL, 
    [LastModificationDateTime]			DATETIME2 NULL,
	CONSTRAINT [PK_dbo.GroupCategory]	PRIMARY KEY CLUSTERED ([GroupCategoryId]),
	CONSTRAINT [UK_GroupCategory]		UNIQUE ([CategoryName])
)
