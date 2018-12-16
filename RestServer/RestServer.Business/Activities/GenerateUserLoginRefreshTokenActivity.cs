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
using RestServer.Entities.DataAccess;
using RestServer.DataAccess.Core.Interfaces;

namespace RestServer.Business.Activities
{
    public class GenerateUserLoginRefreshTokenActivity : ActivityBase<ContextUserIdActivityData, GenerateUserLoginRefreshTokenResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public GenerateUserLoginRefreshTokenActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<GenerateUserLoginRefreshTokenResult> ExecuteAsync(ContextUserIdActivityData requestData)
        {
            var response = new GenerateUserLoginRefreshTokenResult();

            // Fetch the existing user entry to get the mobile number and email address of the user. If the user is not present, then return an error code.
            User existingUser = requestData.UserInContext;

            if (null == existingUser)
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

            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenCreationDateTime = DateTime.UtcNow;
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUserSession = await unitOfWork.UserSessionRepository.GetById(requestData.UserId).ConfigureAwait(false);
                if (null == existingUserSession)
                {
                    // Create a new entry for user session.
                    var firstUserSession = new UserSession
                    {
                        RefreshToken = refreshToken,
                        RefreshTokenCreationDateTime = refreshTokenCreationDateTime,
                        UserId = requestData.UserId
                    };

                    await unitOfWork.UserSessionRepository.InsertAsync(firstUserSession);
                }
                else
                {
                    // Update the existing session with new token
                    existingUserSession.RefreshToken = refreshToken;
                    existingUserSession.RefreshTokenCreationDateTime = refreshTokenCreationDateTime;
                    await unitOfWork.UserSessionRepository.UpdateAsync(existingUserSession);
                }

                await unitOfWork.SaveAsync();
            }

            response.RefreshToken = refreshToken;
            response.RefreshTokenCreationDateTime = refreshTokenCreationDateTime.ToBinary();
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
