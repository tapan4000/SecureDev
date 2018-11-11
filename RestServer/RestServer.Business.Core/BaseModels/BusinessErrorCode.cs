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

        // 10000 - 20000: User management related error codes
        UserWithMobileAlreadyRegistered = 10001,

        UserMobileNumberNotProvided = 10002,

        UserEmailNotProvided = 10003,

        UserEncryptedPasswordNotProvided = 10004,

        UserFirstNameNotProvided = 10005,

        UserLastNameNotProvided = 10006
    }
}
