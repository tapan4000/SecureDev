CREATE TABLE [dbo].[SupplierDevice]
(
	[DeviceId]							INT NOT NULL IDENTITY (1, 1), 
    [SupplierId]						INT NOT NULL, 
    [DeviceCode]						VARCHAR(50) NOT NULL, 
	[DeviceType]						INT NOT NULL,
    [DeviceRegistrationCodeEncrypted]	VARCHAR(50) NOT NULL,
	[CreatedBy]							VARCHAR(50) NOT NULL, 
    [CreationDateTime]					DATETIME2 NOT NULL, 
    [LastModifiedBy]					VARCHAR(50) NULL, 
    [LastModificationDateTime]			DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.SupplierDevice]	PRIMARY KEY CLUSTERED ([DeviceId] ASC),
	CONSTRAINT [UK_SupplierDevice]		UNIQUE ([DeviceCode])
)
