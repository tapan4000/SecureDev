using RestServer.Business.Interfaces.Managers;
using RestServer.Business.Models;
using RestServer.FrontEndService.ContractModels;
using RestServer.FrontEndService.ContractModels.Reponse;
using RestServer.FrontEndService.ContractModels.Request;
using RestServer.FrontEndService.Filters;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestServer.FrontEndService.Controllers
{
    [RoutePrefix("api/anon")]
    [ApplicationValidationFilter]
    public class AnonymousUserController : ApiControllerBase
    {
        private IUserManager userManager;
        private IEventLogger logger;
        private IWorkflowContext workflowContext;

        public AnonymousUserController(IUserManager userManager, IEventLogger logger, IWorkflowContext workflowContext)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.workflowContext = workflowContext;
        }

        [HttpPost]
        [Route("initiateUserRegistration")]
        public async Task<RegisterUserResponseModel> InitiateUserRegistration(RegisterUserRequestModel registerRequest)
        {
            // Usage of API
            // 1) If response status code is 200, all steps were successful. Redirect to the OTP submit page.
            // 2) Else if the response status code is 400, display message to user "Please check the details provided".
            // 3) Else if the response is user already registered (601), then display the message user already registered. Please continue to login.
            // 4) Else if the response is user registration OTP failed (602), redirect the user to OTP screen and display the message "Sending OTP failed". Please retry to send the OTP.
            // 5) Else if the response is user max OTP generation threshold reached (603), display a message to the user max attempt to generate OTP has been reached. Please retry after an hour. This error should mostly come on the OTP submit form where resend OTP option is available.
            // 6) Else if the response is user total OTP generation threshold reached (604), display message that max attempt to generate OTP reached. Please contact administrator.
            // 7) Else if response is user activation period expired (605), display a message to the user and indicate to contact the administrator.
            // 7) Else if the response contains the auth token, take the user to the OTP submit page. Display a message, error occurred while sending OTP, please attempt to send OTP again.
            // 8) Else check if the response contains the user name. In this case the user is already registered, then ask the user to continue login.
            // 9) Else display a message indicating, error occurred during registration. Please reattempt the registration.
            // 10) Independent of whether the result was successful or failed, if the refresh token is present in the response the client should cache the token in a local variable and use it for further login.
            var response = new RegisterUserResponseModel
            {
                Status = (int)PublicStatusCodes.Success
            };

            if (!ModelState.IsValid)
            {
                string errorMessage = string.Format("ModelState Errors: {0}", string.Join(",", ModelState.Values.SelectMany(m => m.Errors).Select(s => s.ErrorMessage)));
                this.logger.LogError(errorMessage);
                response.Status = (int)PublicStatusCodes.BadRequest;
                return response;
            }

            if (null == registerRequest)
            {
                response.Status = (int)PublicStatusCodes.BadRequest;
                return response;
            }

            try
            {
                var result = await this.userManager.AddUser(registerRequest.IsdCode,
                    registerRequest.MobileNumber,
                    registerRequest.Email,
                    registerRequest.FirstName,
                    registerRequest.LastName,
                    registerRequest.UserPasswordHash,
                    this.workflowContext.ApplicationUniqueId);

                // Assign the user auth token and first name that can be used by client to see till what checkpoint the business flow succeeded.
                response.UserAuthToken = result.UserAuthToken;
                response.AuthTokenExpirationDateTime = result.AuthTokenExpirationDateTime;
                response.FirstName = result.FirstName;
                response.RefreshToken = result.UserLoginRefreshToken;
                response.RefreshTokenCreationDateTime = result.RefreshTokenCreationDateTime;

                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserMobileNumberNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserEmailNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserPasswordHashNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserFirstNameNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserLastNameNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.ApplicationUniqueIdNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserIdNotProvided))
                    {
                        // No need to send separate codes for each of the validation failures as the basic sanity check would be done on the mobile.
                        response.Status = (int)PublicStatusCodes.BadRequest;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserWithMobileAlreadyRegistered))
                    {
                        response.Status = (int)PublicStatusCodes.UserWithMobileAlreadyRegistered;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserRegistrationOtpSendFailed))
                    {
                        response.Status = (int)PublicStatusCodes.UserRegistrationOtpSendFailed;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserOtpGenerationWindowThresholdReached))
                    {
                        response.Status = (int)PublicStatusCodes.UserOtpGenerationWindowThresholdReached;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserOtpGenerationTotalThresholdReached))
                    {
                        response.Status = (int)PublicStatusCodes.UserOtpGenerationTotalThresholdReached;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserActivationPeriodExpired))
                    {
                        response.Status = (int)PublicStatusCodes.UserActivationPeriodExpired;
                    }
                    else
                    {
                        response.Status = (int)PublicStatusCodes.SystemError;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogException("Error occurred while initiating the user registration.", ex);
                response.Status = (int)PublicStatusCodes.SystemError;
            }

            return response;
        }

        [HttpPut]
        [Route("loginUser")]
        public async Task<LoginUserResponseModel> LoginUser(LoginUserRequestModel loginRequest)
        {
            // Usage of API
            // 1) If response status code is 200, all steps were successful. Redirect to the dashboard page.
            // 2) Else if the response status code is 400, display message to user "Please check the details provided".
            // 3) Else if the response is user with mobile not found (610), then display the message user not registered.
            // 4) Else if the response is user password mismatch (611), then display the message invalid user credentials.
            // 5) Else if the response is user registration OTP send failed (602), then take the user to the OTP page and display message failed to send OTP. Please try again.
            // 6) Else if the response is user max OTP generation threshold reached (603), take the user to the OTP page and display a message to the user max attempt to generate OTP has been reached. Please retry after an hour.
            // 7) Else if the response is user total OTP generation threshold reached (604), display message that max attempt to generate OTP reached. Please contact administrator.
            // 8) Else if response is user activation period expired (605), display a message to the user and indicate to contact the administrator.
            // 9) Else if response is User pending mobile verification (612), take the user to the OTP page. A notification would have been sent to the user in the background.
            // 10) Else display a message indicating, error occurred during login. Please reattempt the login.
            // 11) Independent of whether the login was success or failure if the response contains the refresh token then save the token on the client side.
            var response = new LoginUserResponseModel
            {
                Status = (int)PublicStatusCodes.Success
            };

            if (!ModelState.IsValid)
            {
                string errorMessage = string.Format("ModelState Errors: {0}", string.Join(",", ModelState.Values.SelectMany(m => m.Errors).Select(s => s.ErrorMessage)));
                this.logger.LogError(errorMessage);
                response.Status = (int)PublicStatusCodes.BadRequest;
                return response;
            }

            if (null == loginRequest)
            {
                response.Status = (int)PublicStatusCodes.BadRequest;
                return response;
            }

            try
            {
                var result = await this.userManager.LoginUser(loginRequest.IsdCode,
                    loginRequest.MobileNumber,
                    loginRequest.UserPasswordHash,
                    this.workflowContext.ApplicationUniqueId);

                // Assign the user auth token and first name that can be used by client to see till what checkpoint the business flow succeeded.
                response.UserAuthToken = result.EncodedSignedToken;
                response.AuthTokenExpirationDateTime = result.AuthTokenExpirationDateTime;
                response.UserLoginRefreshToken = result.UserLoginRefreshToken;
                response.RefreshTokenCreationDateTime = result.RefreshTokenCreationDateTime;

                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserMobileIsdCodeNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserMobileNumberNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserPasswordHashNotProvided))
                    {
                        // No need to send separate codes for each of the validation failures as the basic sanity check would be done on the mobile.
                        response.Status = (int)PublicStatusCodes.BadRequest;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserWithMobileNumberNotFound))
                    {
                        response.Status = (int)PublicStatusCodes.UserWithMobileNumberNotFound;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserPasswordNotMatching))
                    {
                        response.Status = (int)PublicStatusCodes.UserPasswordNotMatching;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserRegistrationOtpSendFailed))
                    {
                        response.Status = (int)PublicStatusCodes.UserRegistrationOtpSendFailed;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserOtpGenerationWindowThresholdReached))
                    {
                        response.Status = (int)PublicStatusCodes.UserOtpGenerationWindowThresholdReached;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserOtpGenerationTotalThresholdReached))
                    {
                        response.Status = (int)PublicStatusCodes.UserOtpGenerationTotalThresholdReached;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserActivationPeriodExpired))
                    {
                        response.Status = (int)PublicStatusCodes.UserActivationPeriodExpired;
                    }
                    else
                    {
                        response.Status = (int)PublicStatusCodes.SystemError;
                    }
                }
                else
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserPendingMobileVerification))
                    {
                        response.Status = (int)PublicStatusCodes.UserPendingMobileVerification;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogException("Error occurred while initiating the user registration.", ex);
                response.Status = (int)PublicStatusCodes.SystemError;
            }

            return response;
        }
    }
}
