CREATE TABLE [dbo].[NotificationMessageTemplate]
(
	[NotificationMessageTemplateId]						INT NOT NULL IDENTITY(1, 1),
	[NotificationModeId]								INT NOT NULL,
	[NotificationMessageTypeId]							INT NOT NULL,
	[LanguageId]										INT NOT NULL,
	[Subject]											NVARCHAR(500) NOT NULL,
	[Body]												NVARCHAR(MAX) NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL,
	CONSTRAINT [PK_dbo.NotificationMessageTemplate]		PRIMARY KEY CLUSTERED ([NotificationMessageTemplateId] ASC),
	CONSTRAINT [UK_NotificationMessageTemplate]			UNIQUE ([NotificationModeId], [NotificationMessageTypeId], [LanguageId])
)
