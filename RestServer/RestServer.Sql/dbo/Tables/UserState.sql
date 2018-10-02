CREATE TABLE [dbo].[UserState]
(
	[UserStateId]										INT			NOT NULL,
	[UserStateName]										VARCHAR(50) NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.UserState]						PRIMARY KEY CLUSTERED ([UserStateId] ASC),
	CONSTRAINT [UK_UserStateName]						UNIQUE ([UserStateName])
)
