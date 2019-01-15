CREATE TABLE [dbo].[LocationCaptureSessionState]
(
	[LocationCaptureSessionStateId]						INT	NOT NULL,
	[LocationCaptureSessionStateName]					VARCHAR(50) NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.LocationCaptureSessionState]		PRIMARY KEY CLUSTERED ([LocationCaptureSessionStateId] ASC),
	CONSTRAINT [UK_LocationCaptureSessionStateName]		UNIQUE ([LocationCaptureSessionStateName])
)
