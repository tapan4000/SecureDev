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
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class ValidateUserRegistrationOtpActivity : ActivityBase<ValidateUserRegistrationOtpRequestData, PopulatedUserBusinessResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public ValidateUserRegistrationOtpActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<PopulatedUserBusinessResult> ExecuteAsync(ValidateUserRegistrationOtpRequestData requestData)
        {
            var response = new PopulatedUserBusinessResult();

            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                if(requestData.User.UserStateId == UserState.None)
                {
                    this.logger.LogError($"The user with id {requestData.User.UserId} is in an invalid state. User state: {requestData.User.UserStateId}");
                    this.Result.IsSuccessful = false;

                    // Return failure result with no error code leading to a 402 response.
                    return response;
                }
                else if(requestData.User.UserStateId != UserState.VerificationPending)
                {
                    this.logger.LogWarning($"The user with id {requestData.User.UserId} is already verified. User state: {requestData.User.UserStateId}");

                    // Return with successful result as no action is needed.
                    return response;
                }

                var existingUserActivationRecord = await unitOfWork.UserActivationRepository.GetById(requestData.User.UserId).ConfigureAwait(false);
                if (null == existingUserActivationRecord)
                {
                    this.logger.LogError($"User activation record for user id {requestData.User.UserId} not found.");
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

                requestData.User.UserStateId = UserState.MobileVerified;
                await unitOfWork.UserRepository.UpdateAsync(requestData.User);
                await unitOfWork.SaveAsync();
            }

            response.User = requestData.User;
            return response;
        }

        protected override bool ValidateRequestData(ValidateUserRegistrationOtpRequestData requestData)
        {
            if (null == requestData.User)
            {
                this.logger.LogError("User id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserParameterNotProvided);
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
