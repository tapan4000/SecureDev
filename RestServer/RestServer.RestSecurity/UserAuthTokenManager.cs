using RestServer.RestSecurity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.RestSecurity.Models;
using RestServer.Core.Extensions;
using Newtonsoft.Json;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration;
using RestServer.Configuration.Models;

namespace RestServer.RestSecurity
{
    public class UserAuthTokenManager : IUserAuthTokenManager
    {
        private IConfigurationHandler configurationHandler;

        private IDataSigner dataSigner;

        public UserAuthTokenManager(IConfigurationHandler configurationHandler, IDataSigner dataSigner)
        {
            this.configurationHandler = configurationHandler;
            this.dataSigner = dataSigner;
        }

        public async Task<KeyValuePair<string, DateTime>> GenerateEncodedSignedToken(int userId, string userUniqueId, string applicationUniqueId)
        {
            if (userId <= 0)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (applicationUniqueId.IsEmpty())
            {
                throw new ArgumentNullException(nameof(applicationUniqueId));
            }

            var userAuthSetting = await this.configurationHandler.GetConfiguration<UserAuthSetting>(ConfigurationConstants.UserAuthSetting);
            var expiryDateTime = DateTime.UtcNow.AddSeconds(userAuthSetting.UserAuthTokenTtlInSeconds);
            var authenticationTokenData = new UserAuthenticationToken
            {
                ApplicationUniqueId = applicationUniqueId,
                UserId = userId,
                UserUniqueId = userUniqueId,
                ExpiryDateTime = expiryDateTime.ToBinary()
            };

            var jsonSerializedToken = JsonConvert.SerializeObject(authenticationTokenData);
            var tokenSigningKey = await this.configurationHandler.GetConfiguration(ConfigurationConstants.UserAuthTokenSigningKey).ConfigureAwait(false);
            var signature = this.dataSigner.GetSignature(jsonSerializedToken, tokenSigningKey);
            var signedToken = new SignedUserAuthenticationToken
            {
                Token = authenticationTokenData,
                Signature = signature
            };

            var encodedSignedToken = this.dataSigner.GetEncodedData(JsonConvert.SerializeObject(signedToken));
            return new KeyValuePair<string, DateTime>(encodedSignedToken, expiryDateTime);
        }

        public async Task<UserAuthenticationToken> VerifyAndExtractDecodedToken(string encodedSignedToken)
        {
            if (encodedSignedToken.IsEmpty())
            {
                throw new ArgumentNullException(nameof(encodedSignedToken));
            }

            var decodedSignedToken = JsonConvert.DeserializeObject<SignedUserAuthenticationToken>(this.dataSigner.GetDecodedData(encodedSignedToken));
            if(null == decodedSignedToken)
            {
                throw new ArgumentException("Decoded signed token cannot be null.");
            }

            if(null == decodedSignedToken.Token)
            {
                throw new ArgumentException("The token property cannot be null.");
            }

            if(decodedSignedToken.Signature.IsEmpty())
            {
                throw new ArgumentException("The signature property cannot be null.");
            }

            if (decodedSignedToken.Token.ApplicationUniqueId.IsEmpty())
            {
                throw new ArgumentException("The application unique id cannot be empty.");
            }

            if (decodedSignedToken.Token.UserId <= 0)
            {
                throw new ArgumentException("The user id cannot be zero.");
            }

            if (decodedSignedToken.Token.ExpiryDateTime <= 0)
            {
                throw new ArgumentException("The expiry date time cannot be 0.");
            }

            var tokenSigningKey = await this.configurationHandler.GetConfiguration(ConfigurationConstants.UserAuthTokenSigningKey).ConfigureAwait(false);
            bool isValidSignature = this.dataSigner.IsValidSignature(JsonConvert.SerializeObject(decodedSignedToken.Token), tokenSigningKey, decodedSignedToken.Signature);
            if (isValidSignature)
            {
                var expiryDate = DateTime.FromBinary(decodedSignedToken.Token.ExpiryDateTime);
                if (DateTime.UtcNow < expiryDate)
                {
                    return decodedSignedToken.Token;
                }
                else
                {
                    throw new UnauthorizedAccessException("The token has expired.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("The signature of the data is not valid.");
            }
        }
    }
}
