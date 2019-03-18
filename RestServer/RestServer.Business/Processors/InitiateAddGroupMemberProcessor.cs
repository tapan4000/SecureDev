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
using RestServer.Entities.Enums;
using RestServer.Business.Activities;
using RestServer.Business.Models.Response;
using RestServer.Notification;
using RestServer.Business.Models;
using RestServer.Core.Helpers;
using RestServer.Core.Extensions;

namespace RestServer.Business.Processors
{
    public class InitiateAddGroupMemberProcessor : ProcessorBase<InitiateAddGroupMemberData, AddGroupMemberProcessorResult>
    {
        public InitiateAddGroupMemberProcessor(IActivityFactory activityFactory, IEventLogger logger) : base(activityFactory, logger)
        {
        }

        protected async override Task<AddGroupMemberProcessorResult> ExecuteAsync(InitiateAddGroupMemberData requestData)
        {
            var initiateAddGroupMemberResult = new AddGroupMemberProcessorResult
            {
                ExpiryPeriodInDays = -1
            };

            // Make a call to add the grounp member
            var addGroupMemberRequest = new AddGroupMemberActivityData
            {
                GroupId = requestData.GroupId,
                AddedMemberIsdCode = requestData.AddedMemberIsdCode,
                AddedMemberMobileNumber = requestData.AddedMemberMobileNumber,
                RequestorUser = requestData.RequestorUser,
                IsAdmin = requestData.IsAdmin,
                IsPrimary = false,
                GroupMemberStateId = (int)GroupMemberStateEnum.PendingAcceptance,
                CanAdminExtendEmergencySessionForSelf = requestData.CanAdminExtendEmergencySessionForSelf,
                CanAdminTriggerEmergencySessionForSelf = requestData.CanAdminTriggerEmergencySessionForSelf,
                GroupPeerEmergencyNotificationModePreferenceId = requestData.GroupPeerEmergencyNotificationModePreferenceId
            };

            // Associate the user to the group.
            var addGroupMemberActivityResult = await this.CreateAndExecuteActivity<AddGroupMemberActivity, AddGroupMemberActivityData, AddGroupMemberActivityResult>(addGroupMemberRequest).ConfigureAwait(false);
            if (!this.Result.IsSuccessful)
            {
                return initiateAddGroupMemberResult;
            }

            initiateAddGroupMemberResult.ExpiryPeriodInDays = addGroupMemberActivityResult.ExpiryPeriodInDays;

            // Send an SMS to the user being added as a group member and show the request in the requesting user's and target user's pending requests area.
            // Fetch the group details to send the notification.
            var getGroupByIdRequest = new GroupIdActivityData
            {
                GroupId = requestData.GroupId
            };

            var getGroupByIdResult = await this.CreateAndExecuteActivity<GetGroupByIdActivity, GroupIdActivityData, PopulatedGroupBusinessResult>(getGroupByIdRequest).ConfigureAwait(false);

            // TODO: Change notification type to SMS
            if (null != addGroupMemberActivityResult.ExistingUser && null != getGroupByIdResult.Group)
            {
                var sendNotificationRequest = new SendNotificationActivityData
                {
                    Recipients = new List<NotificationRecipient>
                    {
                        new NotificationRecipient
                        {
                            EmailId = addGroupMemberActivityResult.ExistingUser.Email,
                            NotificationPreference = NotificationModeEnum.Email
                        }
                    },
                    BodyMergeFields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.RequestorName, requestData.RequestorUser.FirstName),
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.GroupName, getGroupByIdResult.Group.GroupName)
                    },
                    NotificationMessageType = NotificationMessageTypeEnum.GroupJoinRequestCreated,
                    CanNotificationFailureCauseFlowFailure = false
                };
                    
                var sendEmailResult = await this.CreateAndExecuteActivity<SendNotificationActivity, SendNotificationActivityData, SendNotificationActivityResult>(sendNotificationRequest).ConfigureAwait(false);
            }
            else
            {
                // Send SMS to the user mobile number
                var sendNotificationRequest = new SendNotificationActivityData
                {
                    Recipients = new List<NotificationRecipient>
                    {
                        new NotificationRecipient
                        {
                            CompleteMobileNumber = Utility.GetCompleteMobileNumber(requestData.AddedMemberIsdCode, requestData.AddedMemberMobileNumber),
                            NotificationPreference = NotificationModeEnum.Sms
                        }
                    },
                    BodyMergeFields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.RequestorName, requestData.RequestorUser.FirstName),
                        new KeyValuePair<string, string>(NotificationTemplateMergeFieldConstants.GroupName, getGroupByIdResult.Group.GroupName)
                    },
                    NotificationMessageType = NotificationMessageTypeEnum.GroupJoinRequestCreated,
                    CanNotificationFailureCauseFlowFailure = false
                };
                var sendSmsResult = await this.CreateAndExecuteActivity<SendNotificationActivity, SendNotificationActivityData, SendNotificationActivityResult>(sendNotificationRequest).ConfigureAwait(false);
            }

            return initiateAddGroupMemberResult;
        }

        protected override bool ValidateRequestData(InitiateAddGroupMemberData requestData)
        {
            if (requestData.GroupId <= 0)
            {
                this.logger.LogError("Group Id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupIdNotProvided);
                return false;
            }

            if (requestData.AddedMemberIsdCode.IsEmpty())
            {
                this.logger.LogError("Isd code for group member being added is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileIsdCodeNotProvided);
                return false;
            }

            if (requestData.AddedMemberMobileNumber.IsEmpty())
            {
                this.logger.LogError("Mobile number for group member being added is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileNumberNotProvided);
                return false;
            }

            if (null == requestData.RequestorUser)
            {
                this.logger.LogError("Requesting user is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserParameterNotProvided);
                return false;
            }

            if (requestData.GroupPeerEmergencyNotificationModePreferenceId <= 0)
            {
                this.logger.LogError("Notification mode preference is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupPeerEmergencyNotificationModePreferenceNotProvided);
                return false;
            }

            return true;
        }
    }
}
