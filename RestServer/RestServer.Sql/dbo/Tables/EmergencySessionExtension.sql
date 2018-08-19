CREATE TABLE [dbo].[EmergencySessionExtension]
(
	[EmergencySessionExtensionId]						INT NOT NULL IDENTITY (1, 1),
	[EmergencySessionId]								INT NOT NULL, 
    [RequestDateTime]									DATETIME2 NULL, 
    [IsExtensionRequestInProgress]						BIT NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL,
	CONSTRAINT [PK_dbo.EmergencySessionExtension]		PRIMARY KEY CLUSTERED ([EmergencySessionExtensionId])
)
