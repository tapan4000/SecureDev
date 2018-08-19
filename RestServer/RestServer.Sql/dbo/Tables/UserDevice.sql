CREATE TABLE [dbo].[UserDevice]
(
	[UserDeviceId]					INT NOT NULL IDENTITY(1, 1), 
    [UserId]						INT NOT NULL, 
    [DeviceId]						INT NOT NULL, 
    [DeviceFriendlyName]			VARCHAR(50) NULL,
	[CreatedBy]						VARCHAR(50) NOT NULL, 
    [CreationDateTime]				DATETIME2 NOT NULL, 
    [LastModifiedBy]				VARCHAR(50) NULL, 
    [LastModificationDateTime]		DATETIME2 NULL, 
	CONSTRAINT [PK_dbo.UserDevice]	PRIMARY KEY CLUSTERED ([UserDeviceId]),
	CONSTRAINT [UK_UserDevice]		UNIQUE ([UserId], [DeviceId])
)
