IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 0)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 1)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'PendingSyncWithLocationProvider', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 2)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'Active', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 3)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (3, 'Stopped', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 4)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (4, 'Expired', SUSER_SNAME(), GETUTCDATE())

GO