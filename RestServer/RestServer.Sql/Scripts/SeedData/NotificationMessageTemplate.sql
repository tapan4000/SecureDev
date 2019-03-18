DECLARE @EnglishLangId INT
Select @EnglishLangId = LanguageId from [Language] where LanguageName = 'English'

DECLARE @EmailNotificationMode INT
Select @EmailNotificationMode = NotificationModeId from NotificationMode where ModeName = 'Email'

DECLARE @SmsNotificationMode INT
Select @SmsNotificationMode = NotificationModeId from NotificationMode where ModeName = 'Sms'

-- UserWelcomeMessage - 1
IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageTemplate] WHERE [NotificationMessageTypeId] = 1 AND [NotificationModeId] = @EmailNotificationMode AND [LanguageId] = @EnglishLangId)
INSERT [dbo].[NotificationMessageTemplate] ([NotificationMessageTypeId], [NotificationModeId], [LanguageId], [Subject], [Body], [CreatedBy], [CreationDateTime]) 
	VALUES (1, @EmailNotificationMode, @EnglishLangId, 'Welcome', 'Welcome', SUSER_SNAME(), GETUTCDATE())


-- GroupJoinRequestCreated - 2
IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageTemplate] WHERE [NotificationMessageTypeId] = 2 AND [NotificationModeId] = @EmailNotificationMode AND [LanguageId] = @EnglishLangId)
INSERT [dbo].[NotificationMessageTemplate] ([NotificationMessageTypeId], [NotificationModeId], [LanguageId], [Subject], [Body], [CreatedBy], [CreationDateTime]) 
	VALUES (2, @EmailNotificationMode, @EnglishLangId, 'You have a request to join a group.', 'You have a request from {RequestorName} to join the group {GroupName}. Please visit the pending request section to approve/deny the request.', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageTemplate] WHERE [NotificationMessageTypeId] = 2 AND [NotificationModeId] = @SmsNotificationMode AND [LanguageId] = @EnglishLangId)
INSERT [dbo].[NotificationMessageTemplate] ([NotificationMessageTypeId], [NotificationModeId], [LanguageId], [Subject], [Body], [CreatedBy], [CreationDateTime]) 
	VALUES (2, @SmsNotificationMode, @EnglishLangId, 'You have a request to join a group.', 'You have a request from {RequestorName} to join the group {GroupName}. Please visit the pending request section to approve/deny the request.', SUSER_SNAME(), GETUTCDATE())

-- UserRegistrationOtp - 3
IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageTemplate] WHERE [NotificationMessageTypeId] = 3 AND [NotificationModeId] = @EmailNotificationMode AND [LanguageId] = @EnglishLangId)
INSERT [dbo].[NotificationMessageTemplate] ([NotificationMessageTypeId], [NotificationModeId], [LanguageId], [Subject], [Body], [CreatedBy], [CreationDateTime]) 
	VALUES (3, @EmailNotificationMode, @EnglishLangId, 'OTP for registration request.', '{OtpValue} is the OTP for the registration request.', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageTemplate] WHERE [NotificationMessageTypeId] = 3 AND [NotificationModeId] = @SmsNotificationMode AND [LanguageId] = @EnglishLangId)
INSERT [dbo].[NotificationMessageTemplate] ([NotificationMessageTypeId], [NotificationModeId], [LanguageId], [Subject], [Body], [CreatedBy], [CreationDateTime]) 
	VALUES (3, @SmsNotificationMode, @EnglishLangId, '{OtpValue} is the OTP for the registration request.', '{OtpValue} is the OTP for the registration request.', SUSER_SNAME(), GETUTCDATE())

-- EmergencySessionNotificationToAdmins - 4
IF NOT EXISTS (SELECT * FROM [dbo].[NotificationMessageTemplate] WHERE [NotificationMessageTypeId] = 4 AND [NotificationModeId] = @SmsNotificationMode AND [LanguageId] = @EnglishLangId)
INSERT [dbo].[NotificationMessageTemplate] ([NotificationMessageTypeId], [NotificationModeId], [LanguageId], [Subject], [Body], [CreatedBy], [CreationDateTime]) 
	VALUES (4, @SmsNotificationMode, @EnglishLangId, 'Emergency request Raised.', '{RequestorName} has raised an emergency request.', SUSER_SNAME(), GETUTCDATE())
