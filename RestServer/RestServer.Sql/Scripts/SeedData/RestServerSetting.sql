IF NOT EXISTS (SELECT * FROM [dbo].[RestServerSetting] WHERE [Key] = 'UserActivationSetting')
INSERT [dbo].[RestServerSetting] ([Key], [Value], [CreatedBy], [CreationDateTime]) 
	VALUES ('UserActivationSetting', '{
	"MaxUserActivationExpiryPeriodInMinutes": 2880,
	"MaxOtpGenerationAttemptWindowThresholdCount": 5,
	"UserActivationDelayPostMaxOtpGenerationAttemptInMinutes": 60,
	"ActivationCodeMinValue": 1000,
	"ActivationCodeMaxValue": 9999,
	"MaxTotalOtpGenerationThresholdCount": 15
}', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[RestServerSetting] WHERE [Key] = 'UserAuthenticationSetting')
INSERT [dbo].[RestServerSetting] ([Key], [Value], [CreatedBy], [CreationDateTime]) 
	VALUES ('UserAuthSetting', '{
	"UserAuthTokenTtlInSeconds": 14400
}', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[RestServerSetting] WHERE [Key] = 'GlobalSetting')
INSERT [dbo].[RestServerSetting] ([Key], [Value], [CreatedBy], [CreationDateTime]) 
	VALUES ('GlobalSetting', '{
	"MinIocpThreadCountForMaxRedisThroughput": 200
}', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[RestServerSetting] WHERE [Key] = 'GroupGeneralSetting')
INSERT [dbo].[RestServerSetting] ([Key], [Value], [CreatedBy], [CreationDateTime]) 
	VALUES ('GroupGeneralSetting', '{
	"MaxGroupCountPerUser": 20
}', SUSER_SNAME(), GETUTCDATE())
