using RestServer.Business.Core.BaseModels;
using RestServer.Business.Core.Processors;
using RestServer.Business.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Logging.Interfaces;
using RestServer.Business.Models;
using RestServer.Core.Extensions;
using RestServer.Business.Activities;
using RestServer.Notification.Interfaces;
using RestServer.Notification;
using RestServer.Business.Models.Response;
using RestServer.RestSecurity.Models;

namespace RestServer.Business.Processors
{
    public class AddUserRequestProcessor : ProcessorBase<AddUserRequestData, AddUserRequestBusinessResult>
    {
        private INotificationHandler notificationHandler;

        public AddUserRequestProcessor(IActivityFactory activityFactory, IEventLogger logger, INotificationHandler notificationHandler) : base(activityFactory, logger)
        {
            this.notificationHandler = notificationHandler;
        }

        protected async override Task<AddUserRequestBusinessResult> ExecuteAsync(AddUserRequestData requestData)
        {
            var addUserRequestBusinessResult = new AddUserRequestBusinessResult();

            // Make a call to add the user to backend.
           var addUserResult = await this.CreateAndExecuteActivity<AddUserToDataStoreActivity, AddUserRequestData, PopulatedUserBusinessResult>(requestData);

            if (!this.Result.IsSuccessful)
            {
                return addUserRequestBusinessResult;
            }

            addUserRequestBusinessResult.FirstName = addUserResult.User.FirstName;

            // Generate the auth token that can be used by the client for all future communication.
            var generateAuthTokenRequest = new GenerateUserAuthTokenRequestData
            {
                ApplicationUniqueId = requestData.ApplicationUniqueId,
                UserId = addUserResult.User.UserId,
                UserUniqueId = addUserResult.User.UserUniqueId
            };

            var generateAuthTokenResult = await this.CreateAndExecuteActivity<GenerateUserAuthTokenActivity, GenerateUserAuthTokenRequestData, GenerateUserAuthTokenResult>(generateAuthTokenRequest);

            if (!this.Result.IsSuccessful)
            {
                // Not compensating the processor flow, as the user can continue to login page and reenter the credentials to generate the auth token. Post which OTP can be sent.
                return addUserRequestBusinessResult;
            }

            addUserRequestBusinessResult.UserAuthToken = generateAuthTokenResult.EncodedSignedToken;
            addUserRequestBusinessResult.AuthTokenExpirationDateTime = generateAuthTokenResult.AuthTokenExpirationDateTime;

            var sendUserRegistrationOtpRequest = new ContextUserIdActivityData()
            {
                UserId = requestData.User.UserId,
                UserInContext = addUserResult.User
            };

            // Send the activation code to user.
            var sendUserRegistrationOtpResult = await this.CreateAndExecuteActivity<SendUserRegistrationOtpActivity, ContextUserIdActivityData, PopulatedUserBusinessResult>(sendUserRegistrationOtpRequest);

            if (!this.Result.IsSuccessful)
            {
                return addUserRequestBusinessResult;
            }

            // Generate the refresh token
            var generateUserLoginTokenResult = await this.CreateAndExecuteActivity<GenerateUserLoginRefreshTokenActivity, ContextUserIdActivityData, GenerateUserLoginRefreshTokenResult>(sendUserRegistrationOtpRequest);
            addUserRequestBusinessResult.UserLoginRefreshToken = generateUserLoginTokenResult.RefreshToken;
            addUserRequestBusinessResult.RefreshTokenCreationDateTime = generateUserLoginTokenResult.RefreshTokenCreationDateTime;

            return addUserRequestBusinessResult;
        }

        protected override bool ValidateRequestData(AddUserRequestData requestData)
        {
            if (null == requestData.User)
            {
                // this scenario should not happen as User object should always be explicitly created in the controller/processor. So, no need to add any explicit business error that needs
                // to be handled. Any such error should result in a 402 System error.
                throw new ArgumentException("User not populated as add user processor data.");
            }

            if (requestData.User.IsdCode.IsEmpty())
            {
                this.logger.LogError("User mobile ISD code is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileIsdCodeNotProvided);
                return false;
            }

            if (requestData.User.MobileNumber.IsEmpty())
            {
                this.logger.LogError("User mobile is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileNumberNotProvided);
                return false;
            }

            if (requestData.User.Email.IsEmpty())
            {
                this.logger.LogError("User email is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserEmailNotProvided);
                return false;
            }

            if (requestData.User.PasswordHash.IsEmpty())
            {
                this.logger.LogError("User password hash is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserPasswordHashNotProvided);
                return false;
            }

            if (requestData.User.FirstName.IsEmpty())
            {
                this.logger.LogError("User first name is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserFirstNameNotProvided);
                return false;
            }

            if (requestData.User.LastName.IsEmpty())
            {
                this.logger.LogError("User last name is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserLastNameNotProvided);
                return false;
            }

            return true;
        }
    }
}
