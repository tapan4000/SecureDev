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
    [RoutePrefix("api/user")]
    [UserAuthenticationFilter]
    public class UserController : ApiControllerBase
    {
        private IUserManager userManager;
        private IEventLogger logger;
        private IWorkflowContext workflowContext;

        public UserController(IUserManager userManager, IEventLogger logger, IWorkflowContext workflowContext)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.workflowContext = workflowContext;
        }

        [HttpPut]
        [Route("completeUserRegistration")]
        public async Task<ServiceResponseModel> CompleteUserRegistration(CompleteUserRegistrationRequestModel request)
        {
            // Usage of the API
            // 1) If the response code is 200, then the user validation is successful. Thereby redirect the user to home page.
            // 2) Else if the response is user activation record not found (606), then display a message to user to contact the administrator.
            // 3) Else if the response is user activation code mismatch (607), then display a message to user incorrect activation code provided. Thereafter user can attempt to send the SMS again.
            // 4) Else if the response is user with id not found, then clear the auth token from the mobile, and take the user to the registration page and display the message - user registration was not successfull. Please re-attempt registration.
            // 5) Else if the response is bad request (402). Display bad request to the user.
            // 6) Else indicate some error occurred. Please try again.
            var response = new ServiceResponseModel
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

            try
            {
                var result = await this.userManager.CompleteUserRegistration(this.workflowContext.UserId, request.Otp);
                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserActivationRecordNotFound))
                    {
                        // No need to send separate codes for each of the validation failures as the basic sanity check would be done on the mobile.
                        response.Status = (int)PublicStatusCodes.UserActivationRecordNotFound;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserActivationCodeMismatch))
                    {
                        response.Status = (int)PublicStatusCodes.UserActivationCodeMismatch;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserWithIdNotFound))
                    {
                        response.Status = (int)PublicStatusCodes.UserWithIdNotFound;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserIdNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserActivationCodeNotProvided))
                    {
                        response.Status = (int)PublicStatusCodes.BadRequest;
                    }
                    else
                    {
                        response.Status = (int)PublicStatusCodes.SystemError;
                    }
                }
            }
            catch(Exception ex)
            {
                this.logger.LogException("Error occurred while completing the user registration.", ex);
                response.Status = (int)PublicStatusCodes.SystemError;
            }

            return response;
        }

        [HttpGet]
        [Route("resendUserRegistrationOtp")]
        public async Task<ServiceResponseModel> ResendUserRegistrationOtp()
        {
            // Usage of API
            // 1) If response status code is 200, all steps were successful. Display message to user, OTP successfully sent to registered mobile.
            // 2) If the response code os 609 (User already mobile verified), then take the user to the dashboard as registration of the user is already completed.
            // 2) Else if the response status code is 400, display message to user "Please contact support".
            // 4) Else if the response is user registration OTP failed (602), display the message "Sending OTP failed". Please retry to send the OTP.
            // 5) Else if the response is user max OTP generation threshold reached (603), display a message to the user max attempt to generate OTP has been reached. Please retry after an hour.
            // 6) Else if the response is user total OTP generation threshold reached (604), display message that max attempt to generate OTP reached. Please contact administrator.
            // 7) Else if response is user activation period expired (605), display a message to the user and indicate to contact the administrator.
            // 9) Else display a message indicating, error occurred during send OTP. Please retry sending the OTP.
            var response = new ServiceResponseModel
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

            try
            {
                var result = await this.userManager.ResendUserRegistrationOtp(this.workflowContext.UserId);
                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserIdNotProvided))
                    {
                        // No need to send separate codes for each of the validation failures as the basic sanity check would be done on the mobile.
                        response.Status = (int)PublicStatusCodes.BadRequest;
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
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserAlreadyMobileVerified))
                    {
                        response.Status = (int)PublicStatusCodes.UserAlreadyMobileVerified;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogException("Error occurred while resending user registration OTP.", ex);
                response.Status = (int)PublicStatusCodes.SystemError;
            }

            return response;
        }

        [HttpPut]
        [Route("loginWithToken")]
        public async Task<LoginUserResponseModel> LoginUserWithToken(LoginUserWithTokenRequestModel loginWithTokenRequest)
        {
            // Usage of API
            // 1) If response status code is 200, all steps were successful. Redirect to the dashboard page.
            // 2) Else if the response status code is 400, display message to user "Please check the details provided".
            // 3) Else if the response is user with id not found (608), then display the message user not registered.
            // 4) Else if the response is unauthorized (401), then display the message "Unauthorized".
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

            if (null == loginWithTokenRequest)
            {
                response.Status = (int)PublicStatusCodes.BadRequest;
                return response;
            }

            try
            {
                var result = await this.userManager.LoginUserWithToken(this.workflowContext.UserId,
                    this.workflowContext.ApplicationUniqueId,
                    loginWithTokenRequest.RefreshToken,
                    loginWithTokenRequest.RefreshTokenCreationDateTime);

                // Assign the user auth token and first name that can be used by client to see till what checkpoint the business flow succeeded.
                response.UserAuthToken = result.EncodedSignedToken;
                response.AuthTokenExpirationDateTime = result.AuthTokenExpirationDateTime;
                response.UserLoginRefreshToken = result.UserLoginRefreshToken;
                response.RefreshTokenCreationDateTime = result.RefreshTokenCreationDateTime;

                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserLoginRefreshTokenIsNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserLoginRefreshTokenCreationDateTimeNotProvided))
                    {
                        // No need to send separate codes for each of the validation failures as the basic sanity check would be done on the mobile.
                        response.Status = (int)PublicStatusCodes.BadRequest;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserWithIdNotFound))
                    {
                        response.Status = (int)PublicStatusCodes.UserWithIdNotFound;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserSessionByIdNotFound)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserRefreshTokenNotValid))
                    {
                        response.Status = (int)PublicStatusCodes.Unauthorized;
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
