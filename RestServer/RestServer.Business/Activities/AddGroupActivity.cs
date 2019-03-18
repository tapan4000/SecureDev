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
using RestServer.Entities.DataAccess;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration.Models;
using RestServer.Configuration;
using RestServer.Business.Models;
using RestServer.Core.Extensions;
using RestServer.Entities.Enums;
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class AddGroupActivity : ActivityBase<AddGroupRequestData, PopulatedGroupBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private IConfigurationHandler configurationHandler;

        public AddGroupActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory, IConfigurationHandler configurationHandler) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.configurationHandler = configurationHandler;
        }

        protected async override Task<PopulatedGroupBusinessResult> ExecuteAsync(AddGroupRequestData requestData)
        {
            var addGroupResult = new PopulatedGroupBusinessResult();
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                // A user cannot be part of more than 20 groups. This restriction is to avoid the user misusing the feature.
                var groupGeneralSetting = await this.configurationHandler.GetConfiguration<GroupGeneralSetting>(ConfigurationConstants.GroupGeneralSetting);

                // Fetch the number of groups to which the user is associated.
                var groupCount = await unitOfWork.GroupMemberRepository.GetActiveGroupCountByUserId(requestData.UserId);
                if(groupCount >= groupGeneralSetting.MaxGroupCountPerUser)
                {
                    this.Result.AddBusinessError(BusinessErrorCode.MaxGroupCountPerUserReached);
                    this.Result.IsSuccessful = false;
                    return addGroupResult;
                }

                // Check if the default group is being created, then the default group should not be present already. This can happen if the completeUserRegistration flow is called multiple
                // times.
                if(requestData.GroupCategoryId == GroupCategoryEnum.Default)
                {
                    var isDefaultGroupAlreadyCreated = await unitOfWork.GroupMemberRepository.IsUserAlreadyHavingPrimaryGroup(requestData.UserId).ConfigureAwait(false);
                    if (isDefaultGroupAlreadyCreated)
                    {
                        // No need to create the default group.
                        this.logger.LogInformation("As the primary group for user is already created, skipping creation of group.");
                        return addGroupResult;
                    }
                }

                // Even if a user already is part of a group with the same name let the user create the group as we cannot stop a group with same name being created
                // by other users. The other users may then request current user to join their group in which case the user would be part of multiple groups with same name.
                var group = new Group
                {
                    GroupCategoryId = (int)requestData.GroupCategoryId,
                    GroupName = requestData.GroupName,
                    GroupDescription = requestData.GroupDescription,
                    IsPublic = requestData.IsPublic
                };

                addGroupResult.Group = await unitOfWork.GroupRepository.InsertAsync(group).ConfigureAwait(false);
                await unitOfWork.SaveAsync().ConfigureAwait(false);

                if (requestData.IsPublic)
                {
                    // If the group has been created as a public group, make an entry into the Public Group table as well with IsVerified as false.
                    // The verification should be done as a backgroup process post which the details can be updated by the admin.
                    var publicGroup = new PublicGroup
                    {
                        GroupId = addGroupResult.Group.GroupId,
                        IsVerified = false,
                        VerifiedDescription = null,
                        VerifiedGroupCategoryId = (int)GroupCategoryEnum.None,
                        VerifiedTitle = null
                    };

                    await unitOfWork.PublicGroupRepository.InsertAsync(publicGroup).ConfigureAwait(false);
                    await unitOfWork.SaveAsync().ConfigureAwait(false);
                }

                // Add the member requesting the group to be added as the default admin of the group.
                var groupMember = new GroupMember
                {
                    GroupId = addGroupResult.Group.GroupId,
                    UserId = requestData.UserId,
                    GroupMemberStateId = (int)GroupMemberStateEnum.Accepted,
                    CanAdminTriggerEmergencySessionForSelf = true,
                    CanAdminExtendEmergencySessionForSelf = true,
                    GroupPeerEmergencyNotificationModePreferenceId = (int)NotificationModeEnum.Sms,
                    IsAdmin = true,
                    IsPrimary = requestData.IsPrimary
                };

                var insertedGroupMember = await unitOfWork.GroupMemberRepository.InsertAsync(groupMember).ConfigureAwait(false);
                await unitOfWork.SaveAsync().ConfigureAwait(false);
            }

            return addGroupResult;
        }

        protected override bool ValidateRequestData(AddGroupRequestData requestData)
        {
            if (requestData.GroupCategoryId <= 0)
            {
                this.logger.LogError("Group Category id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupCategoryIdNotProvided);
                return false;
            }

            if (requestData.GroupName.IsEmpty())
            {
                this.logger.LogError("Group name is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupNameNotProvided);
                return false;
            }

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
