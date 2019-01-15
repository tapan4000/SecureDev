CREATE TABLE [dbo].[LocationCaptureType]
(
	[LocationCaptureTypeId]									INT NOT NULL, 
    [LocationCaptureTypeName]								VARCHAR(50) NOT NULL, 
	[CreatedBy]												VARCHAR(50) NOT NULL, 
    [CreationDateTime]										DATETIME2 NOT NULL, 
    [LastModifiedBy]										VARCHAR(50) NULL, 
    [LastModificationDateTime]								DATETIME2 NULL,
	CONSTRAINT [PK_dbo.LocationCaptureType]					PRIMARY KEY CLUSTERED ([LocationCaptureTypeId]),
)
