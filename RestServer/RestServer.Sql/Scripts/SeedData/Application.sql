IF NOT EXISTS (SELECT * FROM [dbo].[Application] WHERE [ApplicationId] = 1)
INSERT [dbo].[Application] ([ApplicationId], [ApplicationUniqueId], [CreatedBy], [CreationDateTime]) 
	VALUES (1, '24656349-8bd1-4447-93e2-a576a93b80fb', SUSER_SNAME(), GETUTCDATE())

GO