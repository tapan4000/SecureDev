using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using RestServer.Core.Extensions;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.Logging.Interfaces;
using System;
using System.Threading.Tasks;

namespace RestServer.Business.Activities
{
    public class AddUserToDataStoreActivity : CompensatableActivityBase<AddUserRequestData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private AddUserRequestData requestCopy;

        public AddUserToDataStoreActivity(IUnitOfWorkFactory unitOfWorkFactory, IEventLogger logger) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(AddUserRequestData requestData)
        {
            var addUserResult = new PopulatedUserBusinessResult();
            this.requestCopy = requestData;
            using(var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUser = await unitOfWork.UserRepository.GetUserByMobileNumber(requestData.User.IsdCode, requestData.User.MobileNumber).ConfigureAwait(false);
                if(null != existingUser)
                {
                    this.logger.LogWarning($"The user with mobile {requestData.User.MobileNumber} is already registered.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserWithMobileAlreadyRegistered);
                    this.Result.IsSuccessful = false;
                    addUserResult.User = existingUser;
                    return addUserResult;
                }

                // If the new user needs to be registered, assign the required properties.
                requestData.User.UserStateId = UserState.VerificationPending;
                requestData.User.UserUniqueId = Guid.NewGuid().ToString();

                var insertedUser = await unitOfWork.UserRepository.InsertAsync(requestData.User).ConfigureAwait(false);
                await unitOfWork.SaveAsync().ConfigureAwait(false);
                addUserResult.User = insertedUser;
            }

            return addUserResult;
        }

        protected async override Task CompensateAsync()
        {
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUser = await unitOfWork.UserRepository.GetUserByMobileNumber(this.requestCopy.User.IsdCode, this.requestCopy.User.MobileNumber).ConfigureAwait(false);
                if (null != existingUser)
                {
                    this.logger.LogInformation($"The user with mobile {this.requestCopy.User.MobileNumber} being removed as part of compensation.");
                    await unitOfWork.UserRepository.DeleteAsync(existingUser.UserId).ConfigureAwait(false);
                    await unitOfWork.SaveAsync().ConfigureAwait(false);
                }
                else
                {
                    this.logger.LogInformation($"The user with mobile {this.requestCopy.User.MobileNumber} already removed as part of compensation flow.");
                }
            }
        }

        protected override bool ValidateRequestData(AddUserRequestData requestData)
        {
            if(null == requestData.User)
            {
                // this scenario should not happen as User object should always be explicitly created in the controller/processor. So, no need to add any explicit business error that needs
                // to be handled. Any such error should result in a 402 System error.
                throw new ArgumentException("User not populated as add user activity data.");
            }

            if (requestData.User.IsdCode.IsEmpty())
            {
                this.logger.LogError("User mobile ISD code is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileIsdCodeNotProvided);
                return false;
            }

            if (requestData.User.MobileNumber.IsEmpty())
            {
                this.logger.LogError("User mobile is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserMobileNumberNotProvided);
                return false;
            }

            if (requestData.User.Email.IsEmpty())
            {
                this.logger.LogError("User email is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserEmailNotProvided);
                return false;
            }

            if (requestData.User.PasswordHash.IsEmpty())
            {
                this.logger.LogError("User password hash is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserPasswordHashNotProvided);
                return false;
            }

            if (requestData.User.FirstName.IsEmpty())
            {
                this.logger.LogError("User first name is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserFirstNameNotProvided);
                return false;
            }

            if (requestData.User.LastName.IsEmpty())
            {
                this.logger.LogError("User last name is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserLastNameNotProvided);
                return false;
            }

            return true;
        }
    }
}
