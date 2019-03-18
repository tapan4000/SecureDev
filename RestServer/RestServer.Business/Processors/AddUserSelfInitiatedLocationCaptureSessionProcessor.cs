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
using RestServer.Business.Activities;
using RestServer.Entities.Enums;
using RestServer.Business.Models;
using RestServer.Core.Extensions;
using RestServer.Business.Models.Response;
using RestServer.Notification;

namespace RestServer.Business.Processors
{
    public class AddUserSelfInitiatedLocationCaptureSessionProcessor : ProcessorBase<AddUserInitiatedLocationCaptureSessionData, AddLocationCaptureSessionResult>
    {
        public AddUserSelfInitiatedLocationCaptureSessionProcessor(IActivityFactory activityFactory, IEventLogger logger) : base(activityFactory, logger)
        {
        }

        protected async override Task<AddLocationCaptureSessionResult> ExecuteAsync(AddUserInitiatedLocationCaptureSessionData requestData)
        {
            var addUserSelfInitiatedLocationCaptureResult = new AddLocationCaptureSessionResult();

            // Make a call to add the location capture session to backend.
            var addLocationCaptureSessionRequest = new AddLocationCaptureSessionActivityData
            {
                Title = requestData.CaptureSessionTitle,
                LocationProviderUserId = requestData.LocationProviderUserId,
                LocationCaptureSessionStateId = requestData.LocationCaptureSessionStateId,
                LocationCaptureTypeId = requestData.LocationCaptureTypeId,
                GroupId = requestData.GroupId,
                RequestDateTime = requestData.RequestDateTime,
                LocationCapturePeriodInSeconds = requestData.LocationCapturePeriodInSeconds,
                RequestingUser = requestData.RequestingUser
            };

            var addLocationCaptureSessionResult = await this.CreateAndExecuteActivity<AddLocationCaptureSessionActivity, AddLocationCaptureSessionActivityData, AddLocationCaptureSessionResult>(addLocationCaptureSessionRequest);

            if (!this.Result.IsSuccessful)
            {
                return addUserSelfInitiatedLocationCaptureResult;
            }

            addUserSelfInitiatedLocationCaptureResult.ServerLocationCaptureSessionId = addLocationCaptureSessionResult.ServerLocationCaptureSessionId;

            // Store the location capture record.
            var addUserLocationRequest = new AddLocationActivityData
            {
                ContextUser = requestData.RequestingUser,
                EncryptedLatitude = requestData.EncryptedLatitude,
                EncryptedLongitude = requestData.EncryptedLongitude,
                EncryptedSpeed = requestData.EncryptedSpeed,
                EncryptedAltitude = requestData.EncryptedAltitude,
                Timestamp = requestData.RequestDateTime,
                LocationGenerationType = (requestData.LocationCaptureTypeId == LocationCaptureTypeEnum.PeriodicUpdate || 
                    requestData.LocationCaptureTypeId == LocationCaptureTypeEnum.Emergency) 
                    ? LocationGenerationTypeEnum.CaptureSession : LocationGenerationTypeEnum.Adhoc
            };

            var addUserLocationResult = await this.CreateAndExecuteActivity<AddUserLocationActivity, AddLocationActivityData, RestrictedBusinessResultBase>(addUserLocationRequest);

            if (!this.Result.IsSuccessful)
            {
                return addUserSelfInitiatedLocationCaptureResult;
            }

            // Fetch all the admins of the group against which the user has initiated the emergency session except the user who initiated the session.
            var fetchAdminsNotificationDetailExceptCallingUserRequest = new FetchUsersNotificationDetailRequestData
            {
                GroupId = requestData.GroupId,
                UserId = requestData.RequestingUser.UserId
            };

            var fetchAdminsNotificationDetailExceptCallingUserResult = await this.CreateAndExecuteActivity<FetchAdminsNotificationDetailExcludingUserActivity, FetchUsersNotificationDetailRequestData, FetchUsersNotificationDetailResult>(fetchAdminsNotificationDetailExceptCallingUserRequest);
            if (!this.Result.IsSuccessful)
            {
                return addUserSelfInitiatedLocationCaptureResult;
            }

            if (null == fetchAdminsNotificationDetailExceptCallingUserResult.NotificationDetailForAdmins || !fetchAdminsNotificationDetailExceptCallingUserResult.NotificationDetailForAdmins.Any())
            {
                // Send success status to caller, however, send a status indicating the group does not have any admin who can view this data.
                this.Result.AddBusinessError(BusinessErrorCode.GroupHasNoAdminOtherThanRequestingUser);
                return addUserSelfInitiatedLocationCaptureResult;
            }

            // Send an SMS to all admins of the group except the requesting user regarding the emergency.
            var recipients = new List<NotificationRecipient>();
            foreach(var adminNotificationRecord in fetchAdminsNotificationDetailExceptCallingUserResult.NotificationDetailForAdmins)
            {
                recipients.Add(new NotificationRecipient
                {
                    EmailId = adminNotificationRecord.EmailId,
                    CompleteMobileNumber = adminNotificationRecord.CompleteMobileNumber,
                    NotificationPreference = NotificationModeEnum.Sms
                });
            }

            var sendSmsToAdminsNotificationRequest = new SendNotificationActivityData
            {
                Recipients = recipients,
                BodyMergeFields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.RequestorName, requestData.RequestingUser.FirstName)
                    },
                NotificationMessageType = NotificationMessageTypeEnum.EmergencySessionNotificationToAdmins,
                CanNotificationFailureCauseFlowFailure = false
            };

            var sendSmsToAdminsResult = await this.CreateAndExecuteActivity<SendNotificationActivity, SendNotificationActivityData, SendNotificationActivityResult>(sendSmsToAdminsNotificationRequest).ConfigureAwait(false);

            if (!sendSmsToAdminsResult.IsNotificationSuccessful)
            {
                this.logger.LogError($"Failed to send SMS to Admins for emergency session {addLocationCaptureSessionResult.ServerLocationCaptureSessionId}.");
            }

            return addUserSelfInitiatedLocationCaptureResult;
        }

        protected override bool ValidateRequestData(AddUserInitiatedLocationCaptureSessionData requestData)
        {
            if (null == requestData.RequestingUser)
            {
                this.logger.LogError("Requesting user is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserParameterNotProvided);
                return false;
            }

            if (requestData.CaptureSessionTitle.IsEmpty())
            {
                this.logger.LogError("Location capture session title is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LocationCaptureSessionTitleNotProvided);
                return false;
            }

            if (requestData.LocationProviderUserId <= 0)
            {
                this.logger.LogError("User id for location provider user is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserIdNotProvided);
                return false;
            }

            if (requestData.GroupId <= 0)
            {
                this.logger.LogError("Group id for location capture session is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupIdNotProvided);
                return false;
            }

            if (requestData.LocationCapturePeriodInSeconds <= 0)
            {
                this.logger.LogError("Duration for location capture session is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LocationCaptureSessionDurationNotProvided);
                return false;
            }

            if (requestData.EncryptedLatitude.IsEmpty())
            {
                this.logger.LogError("Latitude not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LatitudeNotProvided);
                return false;
            }

            if (requestData.EncryptedLongitude.IsEmpty())
            {
                this.logger.LogError("Longitude not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LongitudeNotProvided);
                return false;
            }

            return true;
        }
    }
}
