using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models
{
    public enum BusinessErrorCode
    {
        // 1001 - 10000: Common Error Codes
        // 10001 - 20000: User management related error codes
        // 20001 - 30000: Group management related error codes
        // 30001 - 40000: Location management related error codes
        BusinessDataValidationFailed = 1001,

        // 10001 - 20000: User management related error codes
        UserWithMobileAlreadyRegistered = 10001,

        UserMobileNumberNotProvided = 10002,

        UserEmailNotProvided = 10003,

        UserPasswordHashNotProvided = 10004,

        UserFirstNameNotProvided = 10005,

        UserLastNameNotProvided = 10006,

        UserMobileIsdCodeNotProvided = 10007,

        UserRegistrationOtpSendFailed = 10008,

        UserIdNotProvided = 10009,

        UserOtpGenerationWindowThresholdReached = 10010,

        UserWithMobileNumberNotFound = 10011,

        UserAlreadyMobileVerified = 10012,

        ApplicationUniqueIdNotProvided = 10013,

        UserOtpGenerationTotalThresholdReached = 10014,

        UserActivationPeriodExpired = 10015,

        UserActivationRecordNotFound = 10016,

        UserActivationCodeMismatch = 10017,

        UserWithIdNotFound = 10018,

        UserActivationCodeNotProvided = 10019,

        UserPendingMobileVerification = 10020,

        UserPasswordNotMatching = 10021,

        UserLoginRefreshTokenIsNotProvided = 10022,

        UserSessionByIdNotFound = 10023,

        UserRefreshTokenNotValid = 10024,

        UserLoginRefreshTokenCreationDateTimeNotProvided = 10025,

        NotificationModeNotProvided = 10026,

        NotificationMessageNotProvided = 10027,

        NotificationRecipientNotProvided = 10028,

        NotificationTitleNotProvided = 10029,

        UserParameterNotProvided = 10030,

        UserIdForGroupMemberAddRequestNotProvided = 10031,

        // Group management related error codes: 20001 - 30000
        MaxGroupCountPerUserReached = 20001,

        GroupCategoryIdNotProvided = 20002,

        GroupNameNotProvided = 20003,

        MaxUserCountPerGroupReached = 20004,

        UserAlreadyAddedToTargetGroup = 20005,

        GroupIdNotProvided = 20006,

        GroupPeerEmergencyNotificationModePreferenceNotProvided = 20007,

        UserAlreadyPendingAcceptanceToGroupMembership = 20008,

        AddGroupMemberDeniedRequestingUserBlocked = 20009,

        AddGroupMemberDeniedRequestedGroupBlocked = 20010,

        GroupMemberToBeAddedUserIdNotProvided = 20011,

        GroupWithIdNotFound = 20012,

        // 30001 - 40000: Location management related error codes
        LocationCaptureSessionExceededAllowedDuration = 30001,

        UserRequestingLocationNotAssociatedToTargetGroup = 30002,

        LocationProviderUserNotAssociatedToTargetGroup = 30003,

        UserRequestingLocationNotAnAdmin = 30004,
    }
}
