CREATE TABLE [dbo].[EmergencySessionGroupAccess]
(
	[EmergencySessionGroupAccessId]					INT NOT NULL IDENTITY(1, 1),
	EmergencySessionId								INT NOT NULL,
	GroupId											INT NOT NULL,
	[CreatedBy]										VARCHAR(50) NOT NULL, 
    [CreationDateTime]								DATETIME2 NOT NULL, 
    [LastModifiedBy]								VARCHAR(50) NULL, 
    [LastModificationDateTime]						DATETIME2 NULL,
	CONSTRAINT [PK_dbo.EmergencySessionGroupAccess] PRIMARY KEY CLUSTERED ([EmergencySessionGroupAccessId]),
	CONSTRAINT [UK_EmergencySessionGroupAccess]		UNIQUE ([EmergencySessionId], [GroupId])
)
