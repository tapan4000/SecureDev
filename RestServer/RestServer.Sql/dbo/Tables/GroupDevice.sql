CREATE TABLE [dbo].[GroupDevice]
(
	[GroupDeviceId]											INT NOT NULL IDENTITY (1, 1), 
    [GroupId]												INT NOT NULL, 
    [DeviceId]												INT NOT NULL, 
    [IsAdministratorAllowedToTriggerEmergencySession]		BIT NOT NULL, 
    [IsAdministratorAllowedToExtendEmergencySession]		BIT NOT NULL,
	[CreatedBy]												VARCHAR(50) NOT NULL, 
    [CreationDateTime]										DATETIME2 NOT NULL, 
    [LastModifiedBy]										VARCHAR(50) NULL, 
    [LastModificationDateTime]								DATETIME2 NULL,
	CONSTRAINT [PK_dbo.GroupDevice]							PRIMARY KEY CLUSTERED ([GroupDeviceId]),
	CONSTRAINT [UK_GroupDevice]								UNIQUE ([GroupId], [DeviceId])
)
