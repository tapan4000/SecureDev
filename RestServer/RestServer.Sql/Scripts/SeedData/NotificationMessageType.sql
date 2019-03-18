IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageType] WHERE [NotificationMessageTypeId] = 0)
INSERT [dbo].[NotificationMessageType] ([NotificationMessageTypeId], [NotificationMessageTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageType] WHERE [NotificationMessageTypeId] = 1)
INSERT [dbo].[NotificationMessageType] ([NotificationMessageTypeId], [NotificationMessageTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'UserWelcomeMessage', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageType] WHERE [NotificationMessageTypeId] = 2)
INSERT [dbo].[NotificationMessageType] ([NotificationMessageTypeId], [NotificationMessageTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'GroupJoinRequestCreated', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageType] WHERE [NotificationMessageTypeId] = 3)
INSERT [dbo].[NotificationMessageType] ([NotificationMessageTypeId], [NotificationMessageTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (3, 'UserRegistrationOtp', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageType] WHERE [NotificationMessageTypeId] = 4)
INSERT [dbo].[NotificationMessageType] ([NotificationMessageTypeId], [NotificationMessageTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (4, 'EmergencySessionNotificationToAdmins', SUSER_SNAME(), GETUTCDATE())