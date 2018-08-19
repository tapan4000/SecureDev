CREATE TABLE [dbo].[EmergencySessionViewer]
(
	[EmergencySessionViewerId]					INT NOT NULL IDENTITY(1, 1), 
    [EmergencySessionId]						INT NOT NULL, 
    [GroupUserId]								INT NOT NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL, 
	CONSTRAINT [PK_dbo.EmergencySessionViewer]	PRIMARY KEY CLUSTERED ([EmergencySessionViewerId]),
	CONSTRAINT [UK_EmergencySessionViewer]		UNIQUE ([EmergencySessionId], [GroupUserId])
)