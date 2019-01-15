IF NOT EXISTS (SELECT * FROM [dbo].[MembershipTier] WHERE [MembershipTierId] = 1)
INSERT [dbo].[MembershipTier] ([MembershipTierId], [TierName], [EmergencySessionMaxDurationInSeconds], [EmergencySessionAvailabilityInSeconds], [LookoutSessionMaxDurationInSeconds], [LookoutSessionAvailabilityInSeconds], [CreatedBy], [CreationDateTime]) 
	VALUES (1, 'Free', 14400, 604800, 14400, 604800, SUSER_SNAME(), GETUTCDATE())
