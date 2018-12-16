using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels
{
    public class RegisterUserResponseModel : ServiceResponseModel
    {
        [JsonProperty("token")]
        public string UserAuthToken { get; set; }

        [JsonProperty("tokenExpiry")]
        public DateTime AuthTokenExpirationDateTime { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("refreshTime")]
        public long RefreshTokenCreationDateTime { get; set; }
    }
}
