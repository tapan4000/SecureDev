using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models;
using RestServer.Business.Models.Request;
using RestServer.Core.Extensions;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Activities
{
    public class AddUserActivity : CompensatableActivityBase<AddUserRequestData, BusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public AddUserActivity(IUnitOfWorkFactory unitOfWorkFactory, IEventLogger logger) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public async override Task ExecuteAsync(AddUserRequestData requestData)
        {
            using(var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUser = await unitOfWork.UserRepository.GetUserByMobileNumber(requestData.User.MobileNumber).ConfigureAwait(false);
                if(null != existingUser)
                {
                    this.logger.LogWarning($"The user with mobile {requestData.User.MobileNumber} is already registered.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserWithMobileAlreadyRegistered);
                    this.Result.IsSuccessful = false;
                }

                await unitOfWork.UserRepository.InsertAsync(requestData.User).ConfigureAwait(false);
            }
        }

        public override Task<bool> CompensateAsync(AddUserRequestData requestData)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateRequestData(AddUserRequestData requestData)
        {
            if(null == requestData.User)
            {
                // this scenario should not happen as User object should always be explicitly created in the controller/processor. So, no need to add any explicit business error that needs
                // to be handled. Any such error should result in a 402 System error.
                throw new ArgumentException("User not populated as add user activity data.");
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

            if (requestData.User.EncryptedPassword.IsEmpty())
            {
                this.logger.LogError("User encrypted password is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserEncryptedPasswordNotProvided);
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
