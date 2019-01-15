using RestServer.Business.Core.Activities;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration.Models;
using RestServer.Configuration;
using RestServer.Business.Models;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Enums;
using RestServer.DataAccess.Core.Models;
using RestServer.Core.Extensions;

namespace RestServer.Business.Activities
{
    public class AddGroupMemberActivity : ActivityBase<AddGroupMemberActivityData, AddGroupMemberActivityResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private IConfigurationHandler configurationHandler;

        public AddGroupMemberActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory, IConfigurationHandler configurationHandler) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.configurationHandler = configurationHandler;
        }

        protected async override Task<AddGroupMemberActivityResult> ExecuteAsync(AddGroupMemberActivityData requestData)
        {
            var addGroupMemberResult = new AddGroupMemberActivityResult
            {
                ExpiryPeriodInDays = -1
            };

            UserBlockList userBlockList = null;
            User addedUserRecord = null;

            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                // Fetch the user being added based on mobile number.
                addedUserRecord = await unitOfWork.UserRepository.GetUserByMobileNumber(requestData.AddedMemberIsdCode, requestData.AddedMemberMobileNumber).ConfigureAwait(false);
            }

            if(null != addedUserRecord)
            {
                // If the target user has blocked a user or group, then any request initiated by the blocked user will not be initiated.
                using (var unitOfWork = this.unitOfWorkFactory.DocumentDbUnitOfWork)
                {
                    userBlockList = await unitOfWork.UserBlockListRepository.GetUserBlockList(addedUserRecord.UserId).ConfigureAwait(false);
                }
            }

            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                // A user cannot be part of more than 20 groups. This restriction is to avoid the user misusing the feature.
                var groupGeneralSetting = await this.configurationHandler.GetConfiguration<GroupGeneralSetting>(ConfigurationConstants.GroupGeneralSetting);

                // Fetch the number of users associated to the group.
                var userCount = await unitOfWork.GroupMemberRepository.GetActiveUserCountByGroupId(requestData.GroupId);
                if (userCount >= groupGeneralSetting.MaxUserCountPerGroup)
                {
                    this.Result.AddBusinessError(BusinessErrorCode.MaxUserCountPerGroupReached);
                    this.Result.IsSuccessful = false;
                    return addGroupMemberResult;
                }

                if(null != userBlockList)
                {
                    // Check if the requesting user has been blocked.
                    if(null != userBlockList.BlockedUserIds && userBlockList.BlockedUserIds.Any(userId => userId == requestData.RequestorUser.UserId))
                    {
                        this.Result.AddBusinessError(BusinessErrorCode.AddGroupMemberDeniedRequestingUserBlocked);
                        this.Result.IsSuccessful = false;
                        return addGroupMemberResult;
                    }

                    // Check if the requested group has been blocked by the group member being added.
                    if (null != userBlockList.BlockedGroupIds && userBlockList.BlockedGroupIds.Any(groupId => groupId == requestData.GroupId))
                    {
                        this.Result.AddBusinessError(BusinessErrorCode.AddGroupMemberDeniedRequestedGroupBlocked);
                        this.Result.IsSuccessful = false;
                        return addGroupMemberResult;
                    }
                }

                if(null != addedUserRecord)
                {
                    addGroupMemberResult.ExistingUser = addedUserRecord;

                    // Fetch the number of groups associated to the user.
                    var groupCount = await unitOfWork.GroupMemberRepository.GetActiveGroupCountByUserId(addGroupMemberResult.ExistingUser.UserId);

                    // Check if the user is already added to the group.
                    var existingGroupMemberRecord = await unitOfWork.GroupMemberRepository.GetExistingGroupMemberRecord(requestData.GroupId, addedUserRecord.UserId).ConfigureAwait(false);
                    if (null != existingGroupMemberRecord)
                    {
                        if ((existingGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.Accepted)
                            || (existingGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.PendingRequestForUpgradeToAdmin))
                        {
                            this.Result.AddBusinessError(BusinessErrorCode.UserAlreadyAddedToTargetGroup);
                            this.Result.IsSuccessful = false;
                            return addGroupMemberResult;
                        }
                        else if (existingGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.PendingAcceptance)
                        {
                            this.Result.AddBusinessError(BusinessErrorCode.UserAlreadyPendingAcceptanceToGroupMembership);
                            this.Result.IsSuccessful = false;
                            return addGroupMemberResult;
                        }
                        else if ((existingGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.Rejected) 
                            || (existingGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.RequestDeletedByAdmin))
                        {
                            // Make sure the max active group count per user has not been reached.
                            if (groupCount >= groupGeneralSetting.MaxGroupCountPerUser)
                            {
                                this.Result.AddBusinessError(BusinessErrorCode.MaxGroupCountPerUserReached);
                                this.Result.IsSuccessful = false;
                                return addGroupMemberResult;
                            }

                            // In case the target user has rejected the request, the requestor can recreate the request.
                            existingGroupMemberRecord.GroupMemberStateId = (int)GroupMemberStateEnum.PendingAcceptance;
                            await unitOfWork.GroupMemberRepository.UpdateAsync(existingGroupMemberRecord).ConfigureAwait(false);
                            await unitOfWork.SaveAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            this.logger.LogError($"Group membership record for Group {requestData.GroupId} and User {addedUserRecord.UserId} is not is valid state. State: {existingGroupMemberRecord.GroupMemberStateId}");
                            this.Result.IsSuccessful = false;
                            return addGroupMemberResult;
                        }
                    }
                    else
                    {
                        // Make sure the max active group count per user has not been reached.
                        if (groupCount >= groupGeneralSetting.MaxGroupCountPerUser)
                        {
                            this.Result.AddBusinessError(BusinessErrorCode.MaxGroupCountPerUserReached);
                            this.Result.IsSuccessful = false;
                            return addGroupMemberResult;
                        }

                        if (!addedUserRecord.IsGroupMemberRequestSynchronized)
                        {
                            // Even if the group member requests have not yet been synchronized (which is expected to happen during registration and login)
                            // Still we can simply add the record to the group member data store. At the time of synchronization, all requests which are already present
                            // in the group member data store will be ignored.
                            this.logger.LogInformation($"The group members are not yet synchronized for User id {addedUserRecord.UserId}. Going ahead and adding the new entry.");
                        }

                        var groupMember = new GroupMember
                        {
                            GroupId = requestData.GroupId,
                            UserId = addedUserRecord.UserId,
                            GroupMemberStateId = requestData.GroupMemberStateId,
                            CanAdminTriggerEmergencySessionForSelf = requestData.CanAdminTriggerEmergencySessionForSelf,
                            CanAdminExtendEmergencySessionForSelf = requestData.CanAdminExtendEmergencySessionForSelf,
                            GroupPeerEmergencyNotificationModePreferenceId = requestData.GroupPeerEmergencyNotificationModePreferenceId,
                            IsAdmin = requestData.IsAdmin,
                            IsPrimary = requestData.IsPrimary
                        };

                        var insertedGroupMember = await unitOfWork.GroupMemberRepository.InsertAsync(groupMember);
                        await unitOfWork.SaveAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    // If the user has not yet registered, the request should be placed in the AnonymousGroupMemberRequest store. All the requests should be moved to
                    // GroupMember store at the time of user registration or login and corresponding flag indicating the successful sync should be marked. 
                    // So, any user viewing the pending requests should always refer to the GroupMember table. However, the group admins viewing the pending 
                    // member requests for a group should be able to see both GroupMember and AnonymousGroupMemberRequest
                    // related records. If the user being added does not register within 5 days of sending the group membership request, then the request will be abandoned.
                    // Check if the record with the mobile number and group id is already added to the anonymous group member request store.
                    var existingAnonymousGroupMemberRecord = await unitOfWork.AnonymousGroupMemberRepository.GetExistingAnonymousGroupMemberRecord(requestData.AddedMemberIsdCode, requestData.AddedMemberMobileNumber, requestData.GroupId);

                    if(null != existingAnonymousGroupMemberRecord)
                    {
                        if (existingAnonymousGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.PendingAcceptance)
                        {
                            this.Result.AddBusinessError(BusinessErrorCode.UserAlreadyPendingAcceptanceToGroupMembership);
                            this.Result.IsSuccessful = false;
                        }
                        else if ((existingAnonymousGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.Rejected)
                            || (existingAnonymousGroupMemberRecord.GroupMemberStateId == (int)GroupMemberStateEnum.RequestDeletedByAdmin))
                        {
                            // In case the target user has rejected the request, the requestor can recreate the request.
                            addGroupMemberResult.ExpiryPeriodInDays = groupGeneralSetting.AnonymousGroupMembershipExpiryPeriodInDays;
                            existingAnonymousGroupMemberRecord.GroupMemberStateId = requestData.GroupMemberStateId;
                            existingAnonymousGroupMemberRecord.RequestExpiryDateTime = DateTime.UtcNow.AddDays(groupGeneralSetting.AnonymousGroupMembershipExpiryPeriodInDays);
                            existingAnonymousGroupMemberRecord.CanAdminTriggerEmergencySessionForSelf = requestData.CanAdminTriggerEmergencySessionForSelf;
                            existingAnonymousGroupMemberRecord.CanAdminExtendEmergencySessionForSelf = requestData.CanAdminExtendEmergencySessionForSelf;
                            existingAnonymousGroupMemberRecord.GroupPeerEmergencyNotificationModePreferenceId = requestData.GroupPeerEmergencyNotificationModePreferenceId;
                            existingAnonymousGroupMemberRecord.IsAdmin = requestData.IsAdmin;
                            existingAnonymousGroupMemberRecord.IsPrimary = requestData.IsPrimary;
                            await unitOfWork.AnonymousGroupMemberRepository.UpdateAsync(existingAnonymousGroupMemberRecord).ConfigureAwait(false);
                            await unitOfWork.SaveAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            this.logger.LogError($"Group membership record for Group {requestData.GroupId} and User with mobile {requestData.AddedMemberIsdCode}{requestData.AddedMemberMobileNumber} is not is valid state. State: {existingAnonymousGroupMemberRecord.GroupMemberStateId}");
                            this.Result.IsSuccessful = false;
                        }

                        return addGroupMemberResult;
                    }

                    addGroupMemberResult.ExpiryPeriodInDays = groupGeneralSetting.AnonymousGroupMembershipExpiryPeriodInDays;
                    var anonymousGroupMember = new AnonymousGroupMember
                    {
                        GroupId = requestData.GroupId,
                        AnonymousUserIsdCode = requestData.AddedMemberIsdCode,
                        AnonymousUserMobileNumber = requestData.AddedMemberMobileNumber,
                        GroupMemberStateId = requestData.GroupMemberStateId,
                        RequestExpiryDateTime = DateTime.UtcNow.AddDays(groupGeneralSetting.AnonymousGroupMembershipExpiryPeriodInDays),
                        CanAdminTriggerEmergencySessionForSelf = requestData.CanAdminTriggerEmergencySessionForSelf,
                        CanAdminExtendEmergencySessionForSelf = requestData.CanAdminExtendEmergencySessionForSelf,
                        GroupPeerEmergencyNotificationModePreferenceId = requestData.GroupPeerEmergencyNotificationModePreferenceId,
                        IsAdmin = requestData.IsAdmin,
                        IsPrimary = requestData.IsPrimary
                    };

                    await unitOfWork.AnonymousGroupMemberRepository.InsertAsync(anonymousGroupMember).ConfigureAwait(false);
                    await unitOfWork.SaveAsync().ConfigureAwait(false);
                }
            }

            return addGroupMemberResult;
        }

        protected override bool ValidateRequestData(AddGroupMemberActivityData requestData)
        {
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

            if (requestData.GroupId <= 0)
            {
                this.logger.LogError("Group id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupIdNotProvided);
                return false;
            }

            if(null == requestData.RequestorUser)
            {
                this.logger.LogError("Requesting user is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserParameterNotProvided);
                return false;
            }

            if (requestData.GroupPeerEmergencyNotificationModePreferenceId <= 0)
            {
                this.logger.LogError("Group id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupPeerEmergencyNotificationModePreferenceNotProvided);
                return false;
            }

            return true;
        }
    }
}
