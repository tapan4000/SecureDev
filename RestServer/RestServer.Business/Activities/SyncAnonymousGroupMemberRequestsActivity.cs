using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Request;
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
using RestServer.Core.Extensions;

namespace RestServer.Business.Activities
{
    public class SyncAnonymousGroupMemberRequestsActivity : ActivityBase<UserActivityData, RestrictedBusinessResultBase>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private IConfigurationHandler configurationHandler;

        public SyncAnonymousGroupMemberRequestsActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory, IConfigurationHandler configurationHandler) : base(logger)
        {
            // As the sync can happen repeatedly during login process, mark the failure of this activity as ignorable.
            this.CanIgnoreTrackableFailure = true;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.configurationHandler = configurationHandler;
        }

        protected async override Task<RestrictedBusinessResultBase> ExecuteAsync(UserActivityData requestData)
        {
            var response = new RestrictedBusinessResultBase();
            if (requestData.User.IsGroupMemberRequestSynchronized)
            {
                // If the records have already been synchronized, no need to process the flow.
                return response;
            }

            var groupGeneralSetting = await this.configurationHandler.GetConfiguration<GroupGeneralSetting>(ConfigurationConstants.GroupGeneralSetting);

            // 1) Delete any records in AnonymousGroupMember that are rejected, request Cancelled or expired specific to the isd code and mobile number of the user.
            // 2) Delete any records from the AnonymousGroupMember table that is already in the GroupMember table (whether active or inactive as the record in the GroupMember table would be
            //    the latest created record and old record in AnonymousGroupMember can be ignored.
            // 3) Thereafter, if there exist any record in AnonymousGroupMember table specific to the isd code and mobile number of user, then check the group count threshold to see if any
            //    if the group count threshold will be breached by adding the requested member to group. For this, in GroupMember table group by the group id (all records in active state)
            //    and get count for all the groups (filtered by groups in AnonymousGroupMember table specific to user) and check if any group has a count greater than MaxAllowedUsers - 1.
            //    Delete such groups from anonymous group member table and build the log or group id requests deleted.
            // 4) Get the count of groups associated to user in the GroupMember table and see if this count plus the count of records in AnonymousGroupMember table is more than the max
            //    allowed groups per user. If yes, then delete the first excess records (oldest ones).
            // 5) Insert all the records from AnonymousGroupMember table to GroupMember table that still exist in AnonymousGroupMember table.
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                await unitOfWork.GroupRepository.SyncAnonymousGroupMemberRequests(requestData.User.UserId, requestData.User.IsdCode, requestData.User.MobileNumber, groupGeneralSetting.MaxUserCountPerGroup, groupGeneralSetting.MaxGroupCountPerUser);
            }

            return response;
        }

        protected override bool ValidateRequestData(UserActivityData requestData)
        {
            if (null == requestData.User || requestData.User.UserId <= 0 || requestData.User.IsdCode.IsEmpty() || requestData.User.MobileNumber.IsEmpty())
            {
                this.logger.LogError("User object is not populated.");

                // This error should ideally not come up as the details should be provided by the login or register processor. Hence not adding any error code and should return a 402.
                return false;
            }

            return true;
        }
    }
}
