CREATE TABLE [dbo].[MembershipTier]
(
	[MembershipTierId]										INT NOT NULL, 
    [TierName]												VARCHAR(50) NOT NULL, 
    [EmergencySessionMaxDurationInSeconds]					INT NOT NULL, 
	[EmergencySessionAvailabilityInSeconds]					INT NOT NULL, 
	[LookoutSessionMaxDurationInSeconds]					INT NOT NULL,
	[LookoutSessionAvailabilityInSeconds]					INT NOT NULL,
	[CreatedBy]												VARCHAR(50) NOT NULL, 
    [CreationDateTime]										DATETIME2 NOT NULL, 
    [LastModifiedBy]										VARCHAR(50) NULL, 
    [LastModificationDateTime]								DATETIME2 NULL,
	CONSTRAINT [PK_dbo.MembershipTier]						PRIMARY KEY CLUSTERED ([MembershipTierId]),
	CONSTRAINT [UK_MembershipTier]							UNIQUE ([TierName])
)
