using RestServer.Business.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Core.Interfaces.Processors;
using RestServer.Business.Processors;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using RestServer.Business.Activities;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Enums;

namespace RestServer.Business.Managers
{
    public class UserManager : IUserManager
    {
        private IProcessorFactory businessProcessorFactory;

        public UserManager(IProcessorFactory businessProcessorFactory)
        {
            this.businessProcessorFactory = businessProcessorFactory;
        }

        public async Task<AddUserRequestBusinessResult> AddUser(string isdCode, string mobileNumber, string email, string firstName, string lastName, MembershipTierEnum membershipTier, string passwordHash, string applicationUniqueId)
        {
            var addUserRequestProcessor = this.businessProcessorFactory.CreateProcessor<AddUserRequestProcessor, AddUserRequestData, AddUserRequestBusinessResult>();
            var addUserRequestData = new AddUserRequestData
            {
                User = new Entities.DataAccess.User
                {
                    IsdCode = isdCode,
                    MobileNumber = mobileNumber,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PasswordHash = passwordHash,
                    MembershipTierId = membershipTier
                },
                ApplicationUniqueId = applicationUniqueId
            };

            return await addUserRequestProcessor.TrackAndExecuteAsync(addUserRequestData);
        }

        public async Task<BusinessResult> CompleteUserRegistration(User user, int activationCode)
        {
            var validateUserActivationCodeProcessor = this.businessProcessorFactory.CreateProcessor<CompleteUserRegistrationProcessor, ValidateUserRegistrationOtpRequestData, RestrictedBusinessResultBase>();
            var validateUserActivitionRequestData = new ValidateUserRegistrationOtpRequestData
            {
                User = user,
                ActivationCode = activationCode
            };

            return await validateUserActivationCodeProcessor.TrackAndExecuteAsync(validateUserActivitionRequestData);
        }

        public async Task<BusinessResult> ResendUserRegistrationOtp(int userId)
        {
            var resendUserRegistrationOtpProcessor = this.businessProcessorFactory.CreateGenericProcessor<SendUserRegistrationOtpActivity, ContextUserIdActivityData, PopulatedUserBusinessResult>();
            var resendUserRegistrationOtpActivityData = new ContextUserIdActivityData
            {
                UserId = userId,
            };

            return await resendUserRegistrationOtpProcessor.TrackAndExecuteAsync(resendUserRegistrationOtpActivityData);
        }

        public async Task<LoginUserBusinessResult> LoginUser(string isdCode, string mobileNumber, string passwordHash, string applicationUniqueId)
        {
            var loginUserRequestProcessor = this.businessProcessorFactory.CreateProcessor<LoginUserProcessor, LoginProcessorRequestData, LoginUserBusinessResult>();
            var loginUserRequest = new LoginProcessorRequestData
            {
                    IsdCode = isdCode,
                    MobileNumber = mobileNumber,
                    PasswordHash = passwordHash,
                    ApplicationUniqueId = applicationUniqueId
            };

            return await loginUserRequestProcessor.TrackAndExecuteAsync(loginUserRequest);
        }

        public async Task<LoginUserBusinessResult> LoginUserWithToken(int userId, string applicationUniqueId, string refreshToken, long tokenCreationDateTime)
        {
            var loginUserRequestProcessor = this.businessProcessorFactory.CreateProcessor<LoginUserProcessor, LoginProcessorRequestData, LoginUserBusinessResult>();
            var loginUserRequest = new LoginProcessorRequestData
            {
                UserId = userId,
                ApplicationUniqueId = applicationUniqueId,
                RefreshToken = refreshToken,
                TokenCreationDateTime = tokenCreationDateTime,
                IsTokenBasedLogin = true
            };

            return await loginUserRequestProcessor.TrackAndExecuteAsync(loginUserRequest);
        }
    }
}
