using RestServer.Business.Core.Processors;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
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
using RestServer.Entities.DataAccess;

namespace RestServer.Business.Processors
{
    public class LoginUserProcessor : ProcessorBase<LoginProcessorRequestData, LoginUserBusinessResult>
    {
        public LoginUserProcessor(IActivityFactory activityFactory, IEventLogger logger) : base(activityFactory, logger)
        {
        }

        protected async override Task<LoginUserBusinessResult> ExecuteAsync(LoginProcessorRequestData requestData)
        {
            var loginResponse = new LoginUserBusinessResult();

            User contextUser = null;
            if (!requestData.IsTokenBasedLogin)
            {
                var validateUserCredentialRequest = new ValidateUserCredentialsActivityData
                {
                    IsdCode = requestData.IsdCode,
                    MobileNumber = requestData.MobileNumber,
                    PasswordHash = requestData.PasswordHash
                };

                // Validate if the user is valid.
                var validateUserCredentialResult = await this.CreateAndExecuteActivity<ValidateUserCredentialsActivity, ValidateUserCredentialsActivityData, PopulatedUserBusinessResult>(validateUserCredentialRequest);

                if (!this.Result.IsSuccessful)
                {
                    return loginResponse;
                }

                contextUser = validateUserCredentialResult.User;
            }
            else
            {
                var validateUserLoginRefreshTokenActivityRequest = new ValidateUserLoginRefreshTokenActivityData
                {
                    RefreshToken = requestData.RefreshToken,
                    UserId = requestData.UserId,
                    TokenCreationDateTime = requestData.TokenCreationDateTime
                };

                // Validate if the refresh token is valid.
                var validateRefreshTokenResult = await this.CreateAndExecuteActivity<ValidateUserLoginRefreshTokenActivity, ValidateUserLoginRefreshTokenActivityData, PopulatedUserBusinessResult>(validateUserLoginRefreshTokenActivityRequest);

                if (!this.Result.IsSuccessful)
                {
                    return loginResponse;
                }

                contextUser = validateRefreshTokenResult.User;
            }
            

            var sendUserRegistrationOtpRequest = new ContextUserIdActivityData()
            {
                UserId = contextUser.UserId,
                UserInContext = contextUser
            };

            // If the user is pending mobile verification send the OTP to the user.
            if (this.Result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserPendingMobileVerification))
            {
                // Send the activation code to user.
                var sendUserRegistrationOtpResult = await this.CreateAndExecuteActivity<SendUserRegistrationOtpActivity, ContextUserIdActivityData, PopulatedUserBusinessResult>(sendUserRegistrationOtpRequest);
                if (!sendUserRegistrationOtpResult.IsSuccessful)
                {
                    return loginResponse;
                }
            }

            // Generate the auth token that can be used by the client for all future communication.
            var generateAuthTokenRequest = new GenerateUserAuthTokenRequestData
            {
                ApplicationUniqueId = requestData.ApplicationUniqueId,
                UserId = contextUser.UserId,
                UserUniqueId = contextUser.UserUniqueId
            };

            var generateAuthTokenResult = await this.CreateAndExecuteActivity<GenerateUserAuthTokenActivity, GenerateUserAuthTokenRequestData, GenerateUserAuthTokenResult>(generateAuthTokenRequest);

            if (!this.Result.IsSuccessful)
            {
                return loginResponse;
            }

            loginResponse.EncodedSignedToken = generateAuthTokenResult.EncodedSignedToken;
            loginResponse.AuthTokenExpirationDateTime = generateAuthTokenResult.AuthTokenExpirationDateTime;

            // Generate the refresh token
            var generateUserLoginTokenResult = await this.CreateAndExecuteActivity<GenerateUserLoginRefreshTokenActivity, ContextUserIdActivityData, GenerateUserLoginRefreshTokenResult>(sendUserRegistrationOtpRequest);
            loginResponse.UserLoginRefreshToken = generateUserLoginTokenResult.RefreshToken;
            loginResponse.RefreshTokenCreationDateTime = generateUserLoginTokenResult.RefreshTokenCreationDateTime;

            return loginResponse;
        }

        protected override bool ValidateRequestData(LoginProcessorRequestData requestData)
        {
            if (requestData.IsTokenBasedLogin)
            {
                if (requestData.RefreshToken.IsEmpty())
                {
                    this.logger.LogError("User login refresh token is not provided.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserLoginRefreshTokenIsNotProvided);
                    return false;
                }

                if (requestData.UserId <= 0)
                {
                    this.logger.LogError("User id is not provided.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserIdNotProvided);
                    return false;
                }
            }
            else
            {
                if (requestData.IsdCode.IsEmpty())
                {
                    this.logger.LogError("User mobile ISD code is not provided.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserMobileIsdCodeNotProvided);
                    return false;
                }

                if (requestData.MobileNumber.IsEmpty())
                {
                    this.logger.LogError("User mobile is not provided.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserMobileNumberNotProvided);
                    return false;
                }

                if (requestData.PasswordHash.IsEmpty())
                {
                    this.logger.LogError("User password hash is not provided.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserPasswordHashNotProvided);
                    return false;
                }
            }

            return true;
        }
    }
}
