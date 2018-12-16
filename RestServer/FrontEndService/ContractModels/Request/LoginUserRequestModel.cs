using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Request
{
    public class LoginUserRequestModel
    {
        // This should store the ISD code in format Eg. +91, +1 etc.
        [JsonProperty("isdCode")]
        [Required(ErrorMessage = "ISD code is required.")]
        [MaxLength(3, ErrorMessage = "ISD code max size has been exceeded.")]
        public string IsdCode { get; set; }

        [JsonProperty("mobileNumber")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [MaxLength(10, ErrorMessage = "Mobile number max size has been exceeded.")]
        [MinLength(10, ErrorMessage = "Mobile number min size breached.")]
        public string MobileNumber { get; set; }

        [JsonProperty("password")]
        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(50, ErrorMessage = "Password max size has been exceeded.")]
        public string UserPasswordHash { get; set; }
    }
}
