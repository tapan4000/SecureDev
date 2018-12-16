using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.Core.Extensions;
using RestServer.Business.Models;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.Business.Models.Response;

namespace RestServer.Business.Activities
{
    public class ValidateUserCredentialsActivity : ActivityBase<ValidateUserCredentialsActivityData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public ValidateUserCredentialsActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="requestData">The request data.</param>
        /// <returns></returns>
        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(ValidateUserCredentialsActivityData requestData)
        {
            var response = new PopulatedUserBusinessResult();

            User existingUser = null;
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                existingUser = await unitOfWork.UserRepository.GetUserByMobileNumber(requestData.IsdCode, requestData.MobileNumber).ConfigureAwait(false);
                if (null == existingUser)
                {
                    this.logger.LogWarning($"The user with mobile {requestData.MobileNumber} is not present in data store.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserWithMobileNumberNotFound);
                    this.Result.IsSuccessful = false;
                    return response;
                }

            }

            response.User = existingUser;
            if (existingUser.UserStateId == UserState.None)
            {
                // Fail the operation with a 402 as this is not a valid state for user.
                this.Result.IsSuccessful = false;
                return response;
            }

            // Check if the password hash is matching.
            if (!existingUser.PasswordHash.Equals(requestData.PasswordHash))
            {
                this.Result.AddBusinessError(BusinessErrorCode.UserPasswordNotMatching);
                this.Result.IsSuccessful = false;
                return response;
            }

            // Check if the user is pending mobile verification
            if (existingUser.UserStateId == UserState.VerificationPending)
            {
                // If the user is pending verification mark the flow as successful and add the business error code that can be used by consumer to redirect to OTP page.
                this.Result.AddBusinessError(BusinessErrorCode.UserPendingMobileVerification);
                return response;
            }

            return response;
        }

        protected override bool ValidateRequestData(ValidateUserCredentialsActivityData requestData)
        {
            if (requestData.IsdCode.IsEmpty())
            {
                this.logger.LogError("Isd Code is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileIsdCodeNotProvided);
                return false;
            }

            if (requestData.MobileNumber.IsEmpty())
            {
                this.logger.LogError("User mobile is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileNumberNotProvided);
                return false;
            }

            if (requestData.PasswordHash.IsEmpty())
            {
                this.logger.LogError("User password hash is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserPasswordHashNotProvided);
                return false;
            }

            return true;
        }
    }
}
