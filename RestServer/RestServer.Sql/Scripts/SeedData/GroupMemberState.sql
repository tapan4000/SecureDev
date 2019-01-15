IF NOT EXISTS (SELECT * FROM [dbo].[GroupMemberState] WHERE [GroupMemberStateId] = 0)
INSERT [dbo].[GroupMemberState] ([GroupMemberStateId], [GroupMemberStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[GroupMemberState] WHERE [GroupMemberStateId] = 1)
INSERT [dbo].[GroupMemberState] ([GroupMemberStateId], [GroupMemberStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'PendingAcceptance', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[GroupMemberState] WHERE [GroupMemberStateId] = 2)
INSERT [dbo].[GroupMemberState] ([GroupMemberStateId], [GroupMemberStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'PendingRequestForUpgradeToAdmin', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[GroupMemberState] WHERE [GroupMemberStateId] = 3)
INSERT [dbo].[GroupMemberState] ([GroupMemberStateId], [GroupMemberStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (3, 'Accepted', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[GroupMemberState] WHERE [GroupMemberStateId] = 4)
INSERT [dbo].[GroupMemberState] ([GroupMemberStateId], [GroupMemberStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (4, 'Rejected', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[GroupMemberState] WHERE [GroupMemberStateId] = 5)
INSERT [dbo].[GroupMemberState] ([GroupMemberStateId], [GroupMemberStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (5, 'RequestDeletedByAdmin', SUSER_SNAME(), GETUTCDATE())