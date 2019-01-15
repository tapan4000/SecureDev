IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureType] WHERE [LocationCaptureTypeId] = 0)
INSERT [dbo].[LocationCaptureType] ([LocationCaptureTypeId], [LocationCaptureTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureType] WHERE [LocationCaptureTypeId] = 1)
INSERT [dbo].[LocationCaptureType] ([LocationCaptureTypeId], [LocationCaptureTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'PeriodicUpdate', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[LocationCaptureType] WHERE [LocationCaptureTypeId] = 2)
INSERT [dbo].[LocationCaptureType] ([LocationCaptureTypeId], [LocationCaptureTypeName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'Emergency', SUSER_SNAME(), GETUTCDATE())

GO