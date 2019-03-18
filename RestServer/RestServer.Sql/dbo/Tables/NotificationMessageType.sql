CREATE TABLE [dbo].[NotificationMessageType]
(
	[NotificationMessageTypeId]							INT NOT NULL,
	[NotificationMessageTypeName]						VARCHAR(50) NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL,
	CONSTRAINT [PK_dbo.NotificationMessageType]			PRIMARY KEY CLUSTERED ([NotificationMessageTypeId] ASC),
	CONSTRAINT [UK_NotificationMessageTypeName]			UNIQUE ([NotificationMessageTypeName])
)
