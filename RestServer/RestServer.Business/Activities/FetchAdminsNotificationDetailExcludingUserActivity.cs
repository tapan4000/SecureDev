using RestServer.Business.Core.Activities;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Logging.Interfaces;
using RestServer.Business.Models;
using RestServer.DataAccess.Core.Interfaces;

namespace RestServer.Business.Activities
{
    public class FetchAdminsNotificationDetailExcludingUserActivity : ActivityBase<FetchUsersNotificationDetailRequestData, FetchUsersNotificationDetailResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public FetchAdminsNotificationDetailExcludingUserActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<FetchUsersNotificationDetailResult> ExecuteAsync(FetchUsersNotificationDetailRequestData requestData)
        {
            var response = new FetchUsersNotificationDetailResult();

            using(var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var adminNotificationDetailsByGroup = await unitOfWork.GroupRepository.FetchNotificationDetailsForAdminsByGroup(requestData.GroupId).ConfigureAwait(false);
                if(null == adminNotificationDetailsByGroup)
                {
                    return response;
                }

                var adminNotificationDetailsExcludingContextUser = adminNotificationDetailsByGroup.Where(row => row.UserId != requestData.UserId);
                response.NotificationDetailForAdmins = adminNotificationDetailsExcludingContextUser.ToList();
            }

            return response;
        }

        protected override bool ValidateRequestData(FetchUsersNotificationDetailRequestData requestData)
        {
            if (requestData.GroupId <= 0)
            {
                this.logger.LogError("Group id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupIdNotProvided);
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
