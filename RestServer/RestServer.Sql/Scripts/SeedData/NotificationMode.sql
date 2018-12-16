IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMode] WHERE [NotificationModeId] = 0)
INSERT [dbo].[NotificationMode] ([NotificationModeId], [ModeName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMode] WHERE [NotificationModeId] = 1)
INSERT [dbo].[NotificationMode] ([NotificationModeId], [ModeName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'Email', SUSER_SNAME(), GETUTCDATE())


IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMode] WHERE [NotificationModeId] = 2)
INSERT [dbo].[NotificationMode] ([NotificationModeId], [ModeName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'Sms', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMode] WHERE [NotificationModeId] = 3)
INSERT [dbo].[NotificationMode] ([NotificationModeId], [ModeName], [CreatedBy], [CreationDateTime]) 
	VALUES (3, 'PushNotification', SUSER_SNAME(), GETUTCDATE())

GO