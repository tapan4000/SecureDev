using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Request;
using System;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.Business.Models;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.Notification.Interfaces;
using RestServer.Notification;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration.Models;
using RestServer.Configuration;
using RestServer.Business.Models.Response;
using System.Collections.Generic;
using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Entities.Enums;

namespace RestServer.Business.Activities
{
    public class SendUserRegistrationOtpActivity : ActivityBase<ContextUserIdActivityData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private INotificationHandler notificationHandler;

        private IConfigurationHandler configurationHandler;

        public SendUserRegistrationOtpActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory, INotificationHandler notificationHandler, IConfigurationHandler configurationHandler) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.notificationHandler = notificationHandler;
            this.configurationHandler = configurationHandler;
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(ContextUserIdActivityData requestData)
        {
            var response = new PopulatedUserBusinessResult();

            // Fetch the existing user entry to get the mobile number and email address of the user. If the user is not present, then return an error code.
            User existingUser = requestData.UserInContext;

            if(null == existingUser)
            {
                using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
                {
                    existingUser = await unitOfWork.UserRepository.GetById(requestData.UserId).ConfigureAwait(false);
                    if (null == existingUser)
                    {
                        this.Result.IsSuccessful = false;
                        this.logger.LogError($"User with id {requestData.UserId} not found.");

                        // Not adding any business error code as this situation should never occur. If it occurs it should give a 402.
                        return response;
                    }
                }
            }
            
            response.User = existingUser;

            if (existingUser.UserStateId == UserState.None)
            {
                this.logger.LogError($"The user with id {requestData.UserId} is in an invalid state. User state: {existingUser.UserStateId}");
                this.Result.IsSuccessful = false;

                // Return failure result with no error code leading to a 402 response.
                return response;
            }
            else if (existingUser.UserStateId != UserState.VerificationPending)
            {
                this.logger.LogWarning($"The user with id {requestData.UserId} is already verified. User state: {existingUser.UserStateId}");

                // Return with successful result as no action is needed. However, add a business error to notify the consumer that the registration is already completed
                this.Result.AddBusinessError(BusinessErrorCode.UserAlreadyMobileVerified);
                return response;
            }

            // Add the user activation code to the database and increment the activation attempt count.
            var userActivationConfiguration = await this.configurationHandler.GetConfiguration<UserActivationSetting>(ConfigurationConstants.UserActivationSetting);
            var randomNumberGenerator = new Random();
            var activationCode = randomNumberGenerator.Next(1000, 9999);
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUserActivationRecord = await unitOfWork.UserActivationRepository.GetById(requestData.UserId);
                if (null == existingUserActivationRecord)
                {
                    var userActivationRecord = new UserActivation()
                    {
                        ActivationCode = activationCode,
                        CurrentWindowOtpGenerationAttemptCount = 1,
                        TotalOtpGenerationAttemptCount = 1,
                        UserActivationExpiryDateTime = DateTime.UtcNow.AddDays(userActivationConfiguration.MaxUserActivationExpiryPeriodInMinutes),
                        UserId = requestData.UserId,
                    };

                    var newUserActivationRecord = await unitOfWork.UserActivationRepository.InsertAsync(userActivationRecord);
                    await unitOfWork.SaveAsync();
                }
                else
                {
                    if(null != existingUserActivationRecord.UserActivationExpiryDateTime && existingUserActivationRecord.UserActivationExpiryDateTime < DateTime.UtcNow)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.UserActivationPeriodExpired);
                        return response;
                    }

                    if(null != existingUserActivationRecord.NextOtpGenerationWindowStartDateTime && existingUserActivationRecord.NextOtpGenerationWindowStartDateTime > DateTime.UtcNow)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.UserOtpGenerationWindowThresholdReached);
                        return response;
                    }

                    if(existingUserActivationRecord.TotalOtpGenerationAttemptCount >= userActivationConfiguration.MaxTotalOtpGenerationThresholdCount)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.UserOtpGenerationTotalThresholdReached);
                        return response;
                    }

                    bool isActivationAttemptAllowed = true;
                    existingUserActivationRecord.TotalOtpGenerationAttemptCount += 1;
                    if (existingUserActivationRecord.CurrentWindowOtpGenerationAttemptCount >= userActivationConfiguration.MaxOtpGenerationAttemptWindowThresholdCount)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.UserOtpGenerationWindowThresholdReached);
                        existingUserActivationRecord.CurrentWindowOtpGenerationAttemptCount = 0;
                        existingUserActivationRecord.NextOtpGenerationWindowStartDateTime = DateTime.UtcNow.AddMinutes(userActivationConfiguration.UserActivationDelayPostMaxOtpGenerationAttemptInMinutes);
                        isActivationAttemptAllowed = false;
                    }
                    else
                    {
                        existingUserActivationRecord.CurrentWindowOtpGenerationAttemptCount += 1;
                        existingUserActivationRecord.ActivationCode = activationCode;
                    }

                    await unitOfWork.UserActivationRepository.UpdateAsync(existingUserActivationRecord);
                    await unitOfWork.SaveAsync();

                    if (!isActivationAttemptAllowed)
                    {
                        return response;
                    }
                }
            }

            // Send an SMS to the user with the token
            var sendSmsNotificationRequest = new SendNotificationActivityData
            {
                Recipients = new List<NotificationRecipient>
                    {
                        new NotificationRecipient
                        {
                            CompleteMobileNumber = existingUser.CompleteMobileNumber,
                            NotificationPreference = NotificationModeEnum.Sms
                        }
                    },
                BodyMergeFields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.OtpValue, activationCode.ToString())
                    },
                NotificationMessageType = NotificationMessageTypeEnum.UserRegistrationOtp,
                CanNotificationFailureCauseFlowFailure = false
            };

            var sendSmsResult = await this.CreateAndExecuteActivity<SendNotificationActivity, SendNotificationActivityData, SendNotificationActivityResult>(sendSmsNotificationRequest).ConfigureAwait(false);
            if (!sendSmsResult.IsNotificationSuccessful)
            {
                this.logger.LogError("Failed to send the OTP SMS. User must retry sending the SMS using the UI.");

                // Attempt to send the code using email.
                // TODO: Remove the send email functionality as it requires making additional call to database to fetch the user as this is a temporary flow.
                var sendEmailNotificationRequest = new SendNotificationActivityData
                {
                    Recipients = new List<NotificationRecipient>
                    {
                        new NotificationRecipient
                        {
                            EmailId = existingUser.Email,
                            NotificationPreference = NotificationModeEnum.Email
                        }
                    },
                    BodyMergeFields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.OtpValue, activationCode.ToString())
                    },
                    NotificationMessageType = NotificationMessageTypeEnum.UserRegistrationOtp,
                    CanNotificationFailureCauseFlowFailure = false
                };

                var sendEmailResult = await this.CreateAndExecuteActivity<SendNotificationActivity, SendNotificationActivityData, SendNotificationActivityResult>(sendEmailNotificationRequest).ConfigureAwait(false);

                if (!sendEmailResult.IsNotificationSuccessful)
                {
                    this.Result.IsSuccessful = false;
                    this.Result.AddBusinessError(BusinessErrorCode.UserRegistrationOtpSendFailed);
                    return response;
                }
            }

            return response;
        }

        protected override bool ValidateRequestData(ContextUserIdActivityData requestData)
        {
            if (requestData.UserId <= 0)
            {
                this.logger.LogError("User id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserIdNotProvided);
                return false;
            }

            return true;
        }
    }
}
