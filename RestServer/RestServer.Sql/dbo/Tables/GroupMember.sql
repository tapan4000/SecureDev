CREATE TABLE [dbo].[GroupMember]
(
	[GroupMemberId]											INT NOT NULL IDENTITY (1, 1), 
    [GroupId]												INT NOT NULL, 
    [UserId]												INT NOT NULL, 
	[GroupMemberStateId]									INT NOT NULL,
    [CanAdminTriggerEmergencySessionForSelf]				BIT NOT NULL, 
    [CanAdminExtendEmergencySessionForSelf]					BIT NOT NULL, 
    [GroupPeerEmergencyNotificationModePreferenceId]		INT NOT NULL, 
    [IsAdmin]												BIT NOT NULL,
	[IsPrimary]												BIT NOT NULL,
	[CreatedBy]												VARCHAR(50) NOT NULL, 
    [CreationDateTime]										DATETIME2 NOT NULL, 
    [LastModifiedBy]										VARCHAR(50) NULL, 
    [LastModificationDateTime]								DATETIME2 NULL,
	CONSTRAINT [PK_dbo.GroupMember]							PRIMARY KEY CLUSTERED ([GroupMemberId]),
	CONSTRAINT [UK_GroupMember]								UNIQUE ([GroupId], [UserId])
)
