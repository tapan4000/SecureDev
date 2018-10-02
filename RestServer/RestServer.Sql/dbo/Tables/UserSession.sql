﻿CREATE TABLE [dbo].[UserSession]
(
	[UserSessionId]										INT NOT NULL IDENTITY(1, 1),
	[UserId]											INT NOT NULL,
	[RefreshToken]										VARCHAR(50) NOT NULL,
	[RefreshTokenCreationDateTime]						DATETIME NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL, 
	CONSTRAINT [PK_dbo.UserSession]						PRIMARY KEY CLUSTERED ([UserId] ASC),
	CONSTRAINT [UK_UserId]								UNIQUE (UserId)
)