CREATE TABLE [dbo].[User]
(
	[UserId]											INT NOT NULL IDENTITY(1, 1), 
    [PhoneNumber]										VARCHAR(50) NOT NULL, 
    [Email]												VARCHAR(50) NOT NULL, 
    [EncryptedPassword]									VARCHAR(50) NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL, 
	CONSTRAINT [PK_dbo.User]							PRIMARY KEY CLUSTERED ([UserId] ASC),
	CONSTRAINT [UK_UserPhone]							UNIQUE (PhoneNumber)
)
