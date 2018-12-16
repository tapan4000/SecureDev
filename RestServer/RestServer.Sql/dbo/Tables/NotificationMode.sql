CREATE TABLE [dbo].[NotificationMode]
(
	[NotificationModeId]									INT NOT NULL, 
    [ModeName]												VARCHAR(50) NOT NULL, 
	[CreatedBy]												VARCHAR(50) NOT NULL, 
    [CreationDateTime]										DATETIME2 NOT NULL, 
    [LastModifiedBy]										VARCHAR(50) NULL, 
    [LastModificationDateTime]								DATETIME2 NULL,
	CONSTRAINT [PK_dbo.NotificationMode]					PRIMARY KEY CLUSTERED ([NotificationModeId]),
)
