CREATE TABLE [dbo].[AnonymousGroupMember]
(
	[AnonymousGroupMemberId]							INT NOT NULL IDENTITY (1, 1), 
	[GroupId]											INT NOT NULL, 
	[AnonymousUserIsdCode]								VARCHAR(10) NOT NULL, 
    [AnonymousUserMobileNumber]							VARCHAR(50) NOT NULL,
	[GroupMemberStateId]								INT NOT NULL,
	[RequestExpiryDateTime]								DATETIME2 NOT NULL, 
	[CanAdminTriggerEmergencySessionForSelf]			BIT NOT NULL, 
    [CanAdminExtendEmergencySessionForSelf]				BIT NOT NULL, 
    [GroupPeerEmergencyNotificationModePreferenceId]	INT NOT NULL, 
    [IsAdmin]											BIT NOT NULL,
	[IsPrimary]											BIT NOT NULL,
	[CreatedBy]											VARCHAR(50) NOT NULL, 
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL,
	CONSTRAINT [PK_dbo.AnonymousGroupMember]			PRIMARY KEY CLUSTERED ([AnonymousGroupMemberId] ASC),
	CONSTRAINT [UK_AnonymousGroupCompleteMobileNumber]	UNIQUE ([AnonymousUserIsdCode], [AnonymousUserMobileNumber], [GroupId])
)
