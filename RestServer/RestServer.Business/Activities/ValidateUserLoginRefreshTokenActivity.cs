using RestServer.Business.Core.Activities;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Business.Models;
using RestServer.Core.Extensions;

namespace RestServer.Business.Activities
{
    public class ValidateUserLoginRefreshTokenActivity : ActivityBase<ValidateUserLoginRefreshTokenActivityData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public ValidateUserLoginRefreshTokenActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(ValidateUserLoginRefreshTokenActivityData requestData)
        {
            var response = new PopulatedUserBusinessResult();

            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUser = await unitOfWork.UserRepository.GetById(requestData.UserId).ConfigureAwait(false);
                if (null == existingUser)
                {
                    this.logger.LogWarning($"The user with id {requestData.UserId} is not present in data store.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserWithIdNotFound);
                    this.Result.IsSuccessful = false;
                    return response;
                }

                response.User = existingUser;
                var existingUserSession = await unitOfWork.UserSessionRepository.GetById(requestData.UserId).ConfigureAwait(false);
                if(null == existingUserSession)
                {
                    this.logger.LogWarning($"The user session for user id {requestData.UserId} is not present in data store.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserSessionByIdNotFound);
                    this.Result.IsSuccessful = false;
                    return response;
                }

                if (!existingUserSession.RefreshToken.Equals(requestData.RefreshToken))
                {
                    this.logger.LogWarning($"The user refresh token {requestData.RefreshToken} created at {DateTime.FromBinary(requestData.TokenCreationDateTime)} for user id {requestData.UserId} is not valid.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserRefreshTokenNotValid);
                    this.Result.IsSuccessful = false;
                    return response;
                }
            }
            
            return response;
        }

        protected override bool ValidateRequestData(ValidateUserLoginRefreshTokenActivityData requestData)
        {
            if (requestData.RefreshToken.IsEmpty())
            {
                this.logger.LogError("User login refresh token is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserLoginRefreshTokenIsNotProvided);
                return false;
            }

            if (requestData.TokenCreationDateTime <= 0)
            {
                this.logger.LogError("User login refresh token creation date time is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserLoginRefreshTokenCreationDateTimeNotProvided);
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
