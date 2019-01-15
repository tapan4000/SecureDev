IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 0)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 1)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'PendingSyncWithLocationProvider', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureSessionState] WHERE [LocationCaptureSessionStateId] = 2)
INSERT [dbo].[LocationCaptureSessionState] ([LocationCaptureSessionStateId], [LocationCaptureSessionStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'Active', SUSER_SNAME(), GETUTCDATE())

GO