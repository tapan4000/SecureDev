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

namespace RestServer.Business.Activities
{
    public class AddGroupActivity : CompensatableActivityBase<AddGroupRequestData, PopulatedGroupBusinessResult>
    {
        private Group insertedGroup;

        private IUnitOfWorkFactory unitOfWorkFactory;

        private IConfigurationHandler configurationHandler;

        public AddGroupActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory, IConfigurationHandler configurationHandler) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.configurationHandler = configurationHandler;
        }

        protected async override Task CompensateAsync()
        {
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingGroup = await unitOfWork.GroupRepository.GetById(this.insertedGroup.GroupId).ConfigureAwait(false);
                if (null != existingGroup)
                {
                    this.logger.LogInformation($"The group with name {this.insertedGroup.GroupName} and id {this.insertedGroup.GroupId} is being removed as part of compensation.");
                    await unitOfWork.GroupRepository.DeleteAsync(this.insertedGroup.GroupId).ConfigureAwait(false);
                    await unitOfWork.SaveAsync().ConfigureAwait(false);
                }
                else
                {
                    this.logger.LogInformation($"The group with name {this.insertedGroup.GroupName} and id {this.insertedGroup.GroupId} not found as part of compensation.");
                }
            }
        }

        protected async override Task<PopulatedGroupBusinessResult> ExecuteAsync(AddGroupRequestData requestData)
        {
            var addGroupResult = new PopulatedGroupBusinessResult();
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                // A user cannot be part of more than 20 groups. This restriction is to avoid the user misusing the feature.
                var groupGeneralSetting = await this.configurationHandler.GetConfiguration<GroupGeneralSetting>(ConfigurationConstants.GroupGeneralSetting);

                // Fetch the number of groups to which the user is associated.
                var groupCount = await unitOfWork.GroupRepository.GetGroupCountByUserId(requestData.UserId);
                if(groupCount >= groupGeneralSetting.MaxGroupCountPerUser)
                {
                    this.Result.AddBusinessError(BusinessErrorCode.MaxGroupCountPerUserReached);
                    this.Result.IsSuccessful = false;
                    return addGroupResult;
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

                this.insertedGroup = await unitOfWork.GroupRepository.InsertAsync(group).ConfigureAwait(false);
                await unitOfWork.SaveAsync().ConfigureAwait(false);
                addGroupResult.Group = this.insertedGroup;
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
