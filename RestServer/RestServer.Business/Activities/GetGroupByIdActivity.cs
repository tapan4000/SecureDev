using RestServer.Business.Core.Activities;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.Business.Models;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class GetGroupByIdActivity : ActivityBase<GroupIdActivityData, PopulatedGroupBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public GetGroupByIdActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<PopulatedGroupBusinessResult> ExecuteAsync(GroupIdActivityData requestData)
        {
            var getGroupResult = new PopulatedGroupBusinessResult();
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingGroup = await unitOfWork.GroupRepository.GetById(requestData.GroupId).ConfigureAwait(false);
                if (null == existingGroup)
                {
                    this.logger.LogWarning($"The group with id {requestData.GroupId} not found.");
                    this.Result.AddBusinessError(BusinessErrorCode.GroupWithIdNotFound);
                    this.Result.IsSuccessful = false;
                    return getGroupResult;
                }

                getGroupResult.Group = existingGroup;
            }

            return getGroupResult;
        }

        protected override bool ValidateRequestData(GroupIdActivityData requestData)
        {
            if (requestData.GroupId <= 0)
            {
                this.logger.LogError("Group id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupIdNotProvided);
                return false;
            }

            return true;
        }
    }
}
