using RestServer.Business.Core.Processors;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Logging.Interfaces;
using RestServer.Business.Activities;
using RestServer.Entities.Enums;
using RestServer.Business.Models;
using RestServer.Business.Core.BaseModels;

namespace RestServer.Business.Processors
{
    public class CompleteUserRegistrationProcessor : ProcessorBase<ValidateUserRegistrationOtpRequestData, RestrictedBusinessResultBase>
    {
        public CompleteUserRegistrationProcessor(IActivityFactory activityFactory, IEventLogger logger) : base(activityFactory, logger)
        {
        }

        protected async override Task<RestrictedBusinessResultBase> ExecuteAsync(ValidateUserRegistrationOtpRequestData requestData)
        {
            var completeUserRegistrationBusinessResult = new PopulatedUserBusinessResult();

            // Make a call to add the user to backend.
            var validateUserRegistrationOtp = await this.CreateAndExecuteActivity<ValidateUserRegistrationOtpActivity, ValidateUserRegistrationOtpRequestData, PopulatedUserBusinessResult>(requestData);

            if (!this.Result.IsSuccessful)
            {
                return completeUserRegistrationBusinessResult;
            }

            completeUserRegistrationBusinessResult.User = validateUserRegistrationOtp.User;

            // Once the OTP has been validated, create a default group for the user.
            // The group added by default should never be public and it should always be the primary group.
            var addGroupRequestData = new AddGroupRequestData
            {
                GroupCategoryId = GroupCategoryEnum.Default,
                GroupName = completeUserRegistrationBusinessResult.User.FirstName + "'s Family",
                GroupDescription = "DEFAULT",
                IsPublic = false,
                IsPrimary = true,
                UserId = completeUserRegistrationBusinessResult.User.UserId
            };

            // Synchronize the anonymous group requests that might be pending while the user has not registered.
            var userActivityDataRequest = new UserActivityData
            {
                User = completeUserRegistrationBusinessResult.User
            };

            var addGroupActivity = await this.CreateAndExecuteActivity<AddGroupActivity, AddGroupRequestData, PopulatedGroupBusinessResult>(addGroupRequestData);

            if (!this.Result.IsSuccessful)
            {
                return completeUserRegistrationBusinessResult;
            }

            var synchronizeAnonymousGroupRequests = await this.CreateAndExecuteActivity<SyncAnonymousGroupMemberRequestsActivity, UserActivityData, RestrictedBusinessResultBase>(userActivityDataRequest);
            // As the SyncAnonymousGroupMemberRequestsActivity ignores errors, result should always be true. Hence not checking the result.

            return completeUserRegistrationBusinessResult;
        }

        protected override bool ValidateRequestData(ValidateUserRegistrationOtpRequestData requestData)
        {
            if (null == requestData.User)
            {
                this.logger.LogError("User parameter is not provided.");
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
