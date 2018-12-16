CREATE TABLE [dbo].[User]
(
	[UserId]											INT NOT NULL IDENTITY(1, 1), 
	[UserUniqueId]										VARCHAR(50) NOT NULL, 
	[IsdCode]											VARCHAR(10) NOT NULL, 
    [MobileNumber]										VARCHAR(50) NOT NULL, 
    [Email]												VARCHAR(50) NOT NULL, 
	[FirstName]											VARCHAR(50) NOT NULL, 
	[LastName]											VARCHAR(50) NOT NULL, 
    [PasswordHash]										VARCHAR(50) NOT NULL,
	[UserStateId]										INT			NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.User]							PRIMARY KEY CLUSTERED ([UserId] ASC),
	CONSTRAINT [UK_UserMobile]							UNIQUE (MobileNumber),
	CONSTRAINT [UK_UserUniqueId]						UNIQUE (UserUniqueId)
)
