/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT * FROM [dbo].[UserState] WHERE [UserStateId] = 0)
INSERT [dbo].[UserState] ([UserStateId], [UserStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (0, 'None', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[UserState] WHERE [UserStateId] = 1)
INSERT [dbo].[UserState] ([UserStateId], [UserStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'VerificationPending', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[UserState] WHERE [UserStateId] = 2)
INSERT [dbo].[UserState] ([UserStateId], [UserStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (2, 'MobileVerified', SUSER_SNAME(), GETUTCDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[UserState] WHERE [UserStateId] = 3)
INSERT [dbo].[UserState] ([UserStateId], [UserStateName], [CreatedBy], [CreationDateTime]) 
	VALUES (3, 'MobileAndEmailVerified', SUSER_SNAME(), GETUTCDATE())

GO
