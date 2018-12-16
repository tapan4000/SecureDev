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
using RestServer.Business.Models;
using RestServer.Entities.DataAccess;
using RestServer.Business.Models.Response;

namespace RestServer.Business.Activities
{
    public class ValidateUserRegistrationOtpActivity : ActivityBase<ValidateUserRegistrationRequestData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public ValidateUserRegistrationOtpActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(ValidateUserRegistrationRequestData requestData)
        {
            var response = new PopulatedUserBusinessResult();

            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var existingUser = await unitOfWork.UserRepository.GetById(requestData.UserId).ConfigureAwait(false);
                if (null == existingUser)
                {
                    this.logger.LogError($"The user with id {requestData.UserId} not found.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserWithIdNotFound);
                    this.Result.IsSuccessful = false;
                    return response;
                }
                else
                {
                    response.User = existingUser;
                    if(existingUser.UserStateId == UserState.None)
                    {
                        this.logger.LogError($"The user with id {requestData.UserId} is in an invalid state. User state: {existingUser.UserStateId}");
                        this.Result.IsSuccessful = false;

                        // Return failure result with no error code leading to a 402 response.
                        return response;
                    }
                    else if(existingUser.UserStateId != UserState.VerificationPending)
                    {
                        this.logger.LogWarning($"The user with id {requestData.UserId} is already verified. User state: {existingUser.UserStateId}");

                        // Return with successful result as no action is needed.
                        return response;
                    }
                }

                var existingUserActivationRecord = await unitOfWork.UserActivationRepository.GetById(requestData.UserId).ConfigureAwait(false);
                if (null == existingUserActivationRecord)
                {
                    this.logger.LogError($"User activation record for user id {requestData.UserId} not found.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserActivationRecordNotFound);
                    this.Result.IsSuccessful = false;
                    return response;
                }
                
                // Validate if the OTP supplied is the same.
                if(requestData.ActivationCode != existingUserActivationRecord.ActivationCode)
                {
                    this.logger.LogWarning($"The activation code supplied does not match the stored value.");
                    this.Result.AddBusinessError(BusinessErrorCode.UserActivationCodeMismatch);
                    this.Result.IsSuccessful = false;
                    return response;
                }

                existingUser.UserStateId = UserState.MobileVerified;
                await unitOfWork.UserRepository.UpdateAsync(existingUser);
                await unitOfWork.SaveAsync();
            }

            return response;
        }

        protected override bool ValidateRequestData(ValidateUserRegistrationRequestData requestData)
        {
            if (requestData.UserId <= 0)
            {
                this.logger.LogError("User id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserIdNotProvided);
                return false;
            }

            if (requestData.ActivationCode <= 0)
            {
                this.logger.LogError("Activation code is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserActivationCodeNotProvided);
                return false;
            }

            return true;
        }
    }
}
