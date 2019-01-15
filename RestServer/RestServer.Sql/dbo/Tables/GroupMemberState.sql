CREATE TABLE [dbo].[GroupMemberState]
(
	[GroupMemberStateId]										INT			NOT NULL,
	[GroupMemberStateName]										VARCHAR(50) NOT NULL,
	[CreatedBy]													VARCHAR(50) NOT NULL, 
    [CreationDateTime]											DATETIME2 NOT NULL, 
    [LastModifiedBy]											VARCHAR(50) NULL, 
    [LastModificationDateTime]									DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.GroupMemberState]						PRIMARY KEY CLUSTERED ([GroupMemberStateId] ASC),
	CONSTRAINT [UK_GroupMemberStateName]						UNIQUE ([GroupMemberStateName])
)
