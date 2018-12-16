CREATE TABLE [dbo].[UserActivation]
(
	[UserId]											INT	NOT NULL,
	[ActivationCode]									INT NOT NULL,
	[TotalOtpGenerationAttemptCount]					INT NOT NULL,
	[CurrentWindowOtpGenerationAttemptCount]				INT NOT NULL,
	[NextOtpGenerationWindowStartDateTime]					DATETIME2 NOT NULL, 
	[UserActivationExpiryDateTime]						DATETIME2 NOT NULL, 
	[CreatedBy]											VARCHAR(50) NOT NULL,
    [CreationDateTime]									DATETIME2 NOT NULL, 
    [LastModifiedBy]									VARCHAR(50) NULL, 
    [LastModificationDateTime]							DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.UserActivation]					PRIMARY KEY CLUSTERED ([UserId] ASC)
)
