CREATE TABLE [dbo].[EmergencySession]
(
	[EmergencySessionId]					INT NOT NULL IDENTITY (1, 1), 
    [Title]									VARCHAR(50) NOT NULL, 
    [ExpiryDateTime]						DATETIME2 NOT NULL, 
    [FirstNotifiedAdminUserId]				INT NULL, 
    [FirstNotifiedDateTime]					DATETIME2 NULL, 
    [EmergencyTargetUserId]					INT NOT NULL, 
    [IsEmergencyRequestInProgress]			BIT NOT NULL, 
    [CreatedBy]								VARCHAR(50) NOT NULL, 
    [CreationDateTime]						DATETIME2 NOT NULL, 
    [RequestDateTime]						DATETIME2 NULL, 
    [StoppedBy]								VARCHAR(50) NULL, 
    [StopDateTime]							DATETIME2 NULL, 
    [LastModifiedBy]						VARCHAR(50) NULL, 
    [LastModificationDateTime]				DATETIME2 NULL,
	CONSTRAINT [PK_dbo.EmergencySession]	PRIMARY KEY CLUSTERED ([EmergencySessionId])
)
