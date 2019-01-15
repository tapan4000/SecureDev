CREATE TABLE [dbo].[LocationCaptureSession]
(
	[LocationCaptureSessionId]				INT NOT NULL IDENTITY (1, 1), 
    [Title]									VARCHAR(50) NOT NULL, 
    [ExpiryDateTime]						DATETIME2 NOT NULL, 
    [LocationProviderUserId]				INT NOT NULL, 
    [LocationCaptureSessionStateId]			INT NOT NULL,
	[LocationCaptureTypeId]					INT NOT NULL,
	[GroupId]								INT NOT NULL,
	[RequestDateTime]						DATETIME2 NOT NULL, 
	[StoppedBy]								VARCHAR(50) NULL, 
    [StopDateTime]							DATETIME2 NULL, 
    [CreatedBy]								VARCHAR(50) NOT NULL, 
    [CreationDateTime]						DATETIME2 NOT NULL, 
    [LastModifiedBy]						VARCHAR(50) NULL, 
    [LastModificationDateTime]				DATETIME2 NULL,
	CONSTRAINT [PK_dbo.LocationCaptureSession]	PRIMARY KEY CLUSTERED ([LocationCaptureSessionId])
)
