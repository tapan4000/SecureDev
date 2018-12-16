using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Request
{
    public class LoginUserWithTokenRequestModel
    {
        [JsonProperty("token")]
        [Required(ErrorMessage = "Refresh token is required.")]
        [MaxLength(50, ErrorMessage = "Refresh token max size has been exceeded.")]
        public string RefreshToken { get; set; }

        [JsonProperty("refreshTime")]
        [Required(ErrorMessage = "Refresh token date time is required.")]
        public long RefreshTokenCreationDateTime { get; set; }
    }
}
