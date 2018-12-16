using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Request
{
    public class CompleteUserRegistrationRequestModel
    {
        [JsonProperty("otp")]
        [Required(ErrorMessage = "OTP is required.")]
        public int Otp { get; set; }
    }
}
