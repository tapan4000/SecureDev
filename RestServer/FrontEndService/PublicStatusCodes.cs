using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService
{
    public enum PublicStatusCodes
    {
        // Well known status codes
        Success = 200,

        BadRequest = 400,

        Unauthorized = 401,

        SystemError = 402,

        // User account related status codes: 600 range
        UserWithMobileAlreadyRegistered = 601,

        UserRegistrationOtpSendFailed = 602,

        UserOtpGenerationWindowThresholdReached = 603,

        UserOtpGenerationTotalThresholdReached = 604,

        UserActivationPeriodExpired = 605,

        UserActivationRecordNotFound = 606,

        UserActivationCodeMismatch = 607,

        UserWithIdNotFound = 608,

        UserAlreadyMobileVerified = 609,

        UserWithMobileNumberNotFound = 610,

        UserPasswordNotMatching = 611,

        UserPendingMobileVerification = 612,

        // Group related status codes: 700 range
        MaxGroupCountPerUserReached = 701,

        MaxUserCountPerGroupReached = 702,

        AddGroupMemberDenied = 703,

        UserAlreadyAddedToTargetGroup = 704,

        UserAlreadyPendingAcceptanceToGroupMembership = 705,
    }
}
