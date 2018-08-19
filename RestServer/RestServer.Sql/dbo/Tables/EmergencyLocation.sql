CREATE TABLE [dbo].[EmergencyLocation]
(
	[EmergencyLocationId]						INT NOT NULL IDENTITY (1, 1), 
    [LatitudeEncrypted]							VARCHAR(50) NOT NULL, 
    [LongitudeEncrypted]						VARCHAR(50) NOT NULL, 
    [SpeedEncrypted]							VARCHAR(50) NULL, 
    [EmergencySessionId]						INT NOT NULL, 
    [SameLocationReportCount]					INT NOT NULL,
	[CreatedBy]									VARCHAR(50) NOT NULL, 
    [CreationDateTime]							DATETIME2 NOT NULL, 
    [LastModifiedBy]							VARCHAR(50) NULL, 
    [LastModificationDateTime]					DATETIME2 NULL,
	CONSTRAINT [PK_dbo.EmergencyLocation]		PRIMARY KEY CLUSTERED ([EmergencyLocationId])
)
