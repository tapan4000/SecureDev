IF NOT EXISTS (SELECT * FROM [dbo].[GroupCategory] WHERE [GroupCategoryId] = 0)
INSERT [dbo].[GroupCategory] ([GroupCategoryId], [CategoryName], [CategoryDescription], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', 'N/A', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[GroupCategory] WHERE [GroupCategoryId] = 1)
INSERT [dbo].[GroupCategory] ([GroupCategoryId], [CategoryName], [CategoryDescription], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'Personal', 'This category can be used to create groups for family and friends to ensure their security', SUSER_SNAME(), GETUTCDATE())

	
IF NOT EXISTS (SELECT * FROM [dbo].[GroupCategory] WHERE [GroupCategoryId] = 2)
INSERT [dbo].[GroupCategory] ([GroupCategoryId], [CategoryName], [CategoryDescription], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'Security', 'This category can be used to create groups that offer security services to general public.', SUSER_SNAME(), GETUTCDATE())