CREATE TABLE [dbo].[EmergencySessionPublicGroupAccess]
(
	[EmergencySessionPublicGroupAccessId]					INT NOT NULL IDENTITY (1, 1), 
    [GroupId]												INT NOT NULL,
	[EmergencySessionId]									INT NOT NULL,
	[CreatedBy]												VARCHAR(50) NOT NULL, 
    [CreationDateTime]										DATETIME2 NOT NULL, 
    [LastModifiedBy]										VARCHAR(50) NULL, 
    [LastModificationDateTime]								DATETIME2 NULL,
	CONSTRAINT [PK_dbo.EmergencySessionPublicGroupAccess]	PRIMARY KEY CLUSTERED ([EmergencySessionPublicGroupAccessId]),
	CONSTRAINT [UK_EmergencySessionPublicGroupAccess]		UNIQUE ([EmergencySessionId], [GroupId])
)
