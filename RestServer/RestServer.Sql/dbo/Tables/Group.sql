CREATE TABLE [dbo].[Group]
(
	[GroupId]					INT NOT NULL IDENTITY(1, 1), 
	[GroupCategoryId]			INT NOT NULL,
    [GroupName]					VARCHAR(50) NOT NULL, 
    [IsPublic]					BIT NOT NULL,
	[CreatedBy]					VARCHAR(50) NOT NULL, 
    [CreationDateTime]			DATETIME2 NOT NULL, 
    [LastModifiedBy]			VARCHAR(50) NULL, 
    [LastModificationDateTime]	DATETIME2 NULL,
	CONSTRAINT [PK_dbo.Group]	PRIMARY KEY CLUSTERED ([GroupId])
	-- TODO: The public group should have a unique Group name
)
