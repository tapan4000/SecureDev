CREATE PROCEDURE [dbo].[usp_FetchNotificationDetailsForAdminsByGroup]
	@GroupId int
AS
BEGIN
	select u.UserId, u.IsdCode + u.MobileNumber AS CompleteMobileNumber, u.Email as EmailId from [User] u 
	inner join GroupMember gm on u.UserId = gm.UserId
	where gm.GroupId = @GroupId AND gm.IsAdmin = 1
END