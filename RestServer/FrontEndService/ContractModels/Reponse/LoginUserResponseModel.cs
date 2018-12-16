using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Reponse
{
    public class LoginUserResponseModel : ServiceResponseModel
    {
        [JsonProperty("token")]
        public string UserAuthToken { get; set; }

        [JsonProperty("tokenExpiry")]
        public DateTime AuthTokenExpirationDateTime { get; set; }

        [JsonProperty("refreshToken")]
        public string UserLoginRefreshToken { get; set; }

        [JsonProperty("refreshTime")]
        public long RefreshTokenCreationDateTime { get; set; }
    }
}
