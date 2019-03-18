using RestServer.Business.Core.Activities;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.Core.Extensions;
using RestServer.Business.Models;
using RestServer.RestSecurity.Interfaces;
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class GenerateUserAuthTokenActivity : ActivityBase<GenerateUserAuthTokenRequestData, GenerateUserAuthTokenResult>
    {
        private IUserAuthTokenManager userAuthTokenManager;

        public GenerateUserAuthTokenActivity(IActivityFactory activityFactory, IEventLogger logger, IUserAuthTokenManager userAuthTokenManager) : base(activityFactory, logger)
        {
            this.userAuthTokenManager = userAuthTokenManager;
        }

        protected async override Task<GenerateUserAuthTokenResult> ExecuteAsync(GenerateUserAuthTokenRequestData requestData)
        {
            var generateTokenResult = new GenerateUserAuthTokenResult();
            
            // Make use of user id in the token as the user id can directly be used to fetch records in other tables as it should be the foreign key in other tables.
            // Additionally, it can be used to quickly fetch record in User table as it is the primary key for User table.
            var tokenDatePair = await this.userAuthTokenManager.GenerateEncodedSignedToken(requestData.UserId, requestData.UserUniqueId, requestData.ApplicationUniqueId);
            generateTokenResult.EncodedSignedToken = tokenDatePair.Key;
            generateTokenResult.AuthTokenExpirationDateTime = tokenDatePair.Value;
            return generateTokenResult;
        }

        protected override bool ValidateRequestData(GenerateUserAuthTokenRequestData requestData)
        {
            if(null == requestData)
            {
                throw new ArgumentException("The Auth token cannot be null.");
            }

            if (requestData.UserId <= 0)
            {
                this.logger.LogError("User id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserIdNotProvided);
                return false;
            }

            if (requestData.ApplicationUniqueId.IsEmpty())
            {
                this.logger.LogError("Application unique id is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.ApplicationUniqueIdNotProvided);
                return false;
            }

            return true;
        }
    }
}
