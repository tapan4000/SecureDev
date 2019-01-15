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
using RestServer.Business.Models;

namespace RestServer.Business.Activities
{
    public class GetUserByIdActivity : ActivityBase<UserIdActivityData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public GetUserByIdActivity(IUnitOfWorkFactory unitOfWorkFactory, IEventLogger logger) : base(logger)
        {
        }

        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(UserIdActivityData requestData)
        {
            var getUserResult = new PopulatedUserBusinessResult();
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUser = await unitOfWork.UserRepository.GetById(requestData.UserId).ConfigureAwait(false);
                if (null == existingUser)
                {
                    this.logger.LogWarning($"The user with id {requestData.UserId} not found.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserWithIdNotFound);
                    this.Result.IsSuccessful = false;
                    return getUserResult;
                }

                getUserResult.User = existingUser;
            }

            return getUserResult;
        }

        protected override bool ValidateRequestData(UserIdActivityData requestData)
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
