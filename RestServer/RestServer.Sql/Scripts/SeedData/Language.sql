IF NOT EXISTS (SELECT * FROM [dbo].[Language] WHERE [LanguageId] = 1)
INSERT [dbo].[Language] ([LanguageId], [LanguageName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'English', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[Language] WHERE [LanguageId] = 2)
INSERT [dbo].[Language] ([LanguageId], [LanguageName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'Hindi', SUSER_SNAME(), GETUTCDATE())