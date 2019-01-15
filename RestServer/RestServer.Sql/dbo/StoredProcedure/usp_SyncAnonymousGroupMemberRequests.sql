CREATE PROCEDURE [dbo].[usp_SyncAnonymousGroupMemberRequests]
	@UserMobileIsdCode varchar(10),
	@UserMobileNumber varchar(10),
	@UserId int,
	@MaxUserCountPerGroup int,
	@MaxGroupCountPerUser int
AS
BEGIN
DECLARE @TargetUserId int
DECLARE @CurrentDateTime DATETIME
SET @CurrentDateTime = GETDATE()

DECLARE @TransactionName VARCHAR(20)
SET @TransactionName = 'SyncAnonymousGroupMemberRequests'

DECLARE @DeletedGroups TABLE
(
	GroupId INT,
	UserIsdCode VARCHAR(10),
	UserMobileNumber VARCHAR(50)
);

DECLARE @UsersGroupMembershipCount INT;
SET @UsersGroupMembershipCount = 0;

DECLARE @LogToPrint VARCHAR(MAX)
SET @LogToPrint = ''

BEGIN TRY
	BEGIN TRANSACTION @TransactionName -- Creating transaction to make sure that after insertion of records in GroupMember table if exception occurs then same records are not inserted again.
	IF NOT EXISTS(SELECT 1 from AnonymousGroupMember where AnonymousUserIsdCode = @UserMobileIsdCode AND AnonymousUserMobileNumber = @UserMobileNumber)
	BEGIN
		UPDATE [User]
		set IsGroupMemberRequestSynchronized = 1
		where UserId = @UserId 
		
		COMMIT TRANSACTION @TransactionName
		SELECT @LogToPrint
		RETURN 
	END

	-- Delete all the requests which are either rejected by the user (In case user registered but anonymous group member records were not synced) or if the request was deleted by the admin.
	DELETE FROM AnonymousGroupMember where 
	AnonymousUserIsdCode = @UserMobileIsdCode AND AnonymousUserMobileNumber = @UserMobileNumber AND GroupMemberStateId IN (SELECT GroupMemberStateId FROM GroupMemberState WHERE GroupMemberStateName in ('Rejected', 'RequestDeletedByAdmin'))

	-- Delete all the requests which are expired.
	DELETE FROM AnonymousGroupMember where
	AnonymousUserIsdCode = @UserMobileIsdCode AND AnonymousUserMobileNumber = @UserMobileNumber AND (@CurrentDateTime >= RequestExpiryDateTime)

	-- Delete any records from the AnonymousGroupMember table that is already in the GroupMember table (whether active or inactive as the record in the GroupMember table would be
    -- the latest created record and old record in AnonymousGroupMember can be ignored.
	DELETE AGM
	FROM AnonymousGroupMember AGM
	inner join GroupMember GM on AGM.GroupId = GM.GroupId
	where GM.UserId = @UserId AND AGM.AnonymousUserIsdCode = @UserMobileIsdCode AND AGM.AnonymousUserMobileNumber = @UserMobileNumber
		AND GM.GroupId IS NOT NULL

	-- Thereafter, if there exist any record in AnonymousGroupMember table specific to the isd code and mobile number of user, then check the group count threshold to see if any
    -- group count threshold will be breached by adding the requested member to group. For this, in GroupMember table group by the group id (all records in active state)
    -- and get count for all the groups (filtered by groups in AnonymousGroupMember table specific to user) and check if any group has a count greater than MaxAllowedUsers - 1.
    -- Delete such groups from anonymous group member table and build the log or group id requests deleted.
	-- This scenario can occur if the user attempted to add the group member whose request is already in AnonymousGroupMember and the user has registered however, the requests have not 
	-- yet been synched. In ideal business scenario this should not happen as the mobile app should prevent adding the same group member twice, however, if the call is made through Web API
	-- this needs to be checked.
	DELETE FROM AnonymousGroupMember
	OUTPUT deleted.GroupId, deleted.AnonymousUserIsdCode, deleted.AnonymousUserMobileNumber into @DeletedGroups
	where GroupId in 
	(SELECT GroupId from GroupMember
	where UserId = @UserId
	GROUP BY GroupId
	HAVING COUNT(1) > @MaxUserCountPerGroup - 1)
	
	IF EXISTS(SELECT top 1 1 from @DeletedGroups)
	BEGIN
		SET @LogToPrint = @LogToPrint + 'Deleting (GroupId:UserId) from AnonymousGroupMember table as the records are already present in the GroupMember table : '
		Select @LogToPrint = @LogToPrint + '(' + GroupId + ':' + UserIsdCode + UserMobileNumber + '),'
		from @DeletedGroups

		DELETE from @DeletedGroups
	END
	
	-- Get the count of groups associated to user in the GroupMember table and see if this count plus the count of records in AnonymousGroupMember table is more than the max
    -- allowed groups per user. If yes, then delete the first excess records (oldest ones).
	SELECT @UsersGroupMembershipCount = COUNT(1) 
	from GroupMember
	where UserId = @UserId

	SELECT @UsersGroupMembershipCount = @UsersGroupMembershipCount + COUNT(1)
	from AnonymousGroupMember
	where AnonymousUserIsdCode = @UserMobileIsdCode AND AnonymousUserMobileNumber = @UserMobileNumber

	if(@UsersGroupMembershipCount > @MaxGroupCountPerUser)
	BEGIN
		DECLARE @GroupCountPerUserThresholdBreachingRecordCount INT
		SET @GroupCountPerUserThresholdBreachingRecordCount= @UsersGroupMembershipCount - @MaxGroupCountPerUser; 
		DELETE TOP (@GroupCountPerUserThresholdBreachingRecordCount) from AnonymousGroupMember
		OUTPUT deleted.GroupId, deleted.AnonymousUserIsdCode, deleted.AnonymousUserMobileNumber into @DeletedGroups
		where AnonymousUserIsdCode = @UserMobileIsdCode AND AnonymousUserMobileNumber = @UserMobileNumber

		IF EXISTS(SELECT TOP 1 1 from @DeletedGroups)
		BEGIN
			SET @LogToPrint = @LogToPrint + 'Deleting ' + @GroupCountPerUserThresholdBreachingRecordCount + ' records from AnonymousGroupMember table for User Mobile' + @UserMobileIsdCode + @UserMobileNumber + '. Group Id: ('
			Select @LogToPrint = @LogToPrint + ',' + GroupId from @DeletedGroups
			SET @LogToPrint = @LogToPrint + ')'
		END
	END

	-- Insert all remaining records from AnonymousGroupMember table to GroupMember table that still exist in AnonymousGroupMember table.
	INSERT INTO GroupMember (GroupId, UserId, GroupMemberStateId, CanAdminTriggerEmergencySessionForSelf, CanAdminExtendEmergencySessionForSelf, GroupPeerEmergencyNotificationModePreferenceId, IsAdmin, IsPrimary, CreatedBy, CreationDateTime)
	Select GroupId, @UserId, GroupMemberStateId, CanAdminTriggerEmergencySessionForSelf, CanAdminExtendEmergencySessionForSelf, GroupPeerEmergencyNotificationModePreferenceId, IsAdmin, IsPrimary, CreatedBy, CreationDateTime
	from AnonymousGroupMember

	-- Mark the user flag as sync complete
	UPDATE [User]
	set IsGroupMemberRequestSynchronized = 1
	where UserId = @UserId

	-- Delete all the records from AnonymousGroupMember table
	DELETE from AnonymousGroupMember where AnonymousUserIsdCode = @UserMobileIsdCode AND AnonymousUserMobileNumber = @UserMobileNumber

	COMMIT TRANSACTION @TransactionName
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0 
		ROLLBACK TRAN @TransactionName;
	THROW
END CATCH

	SELECT @LogToPrint
END
