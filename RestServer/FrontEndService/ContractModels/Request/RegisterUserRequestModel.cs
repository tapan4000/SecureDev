using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels
{
    public class RegisterUserRequestModel
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

        [JsonProperty("email")]
        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(50, ErrorMessage = "Email size has been exceeded.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid Email Id")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name max size has been exceeded.")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name max size has been exceeded.")]
        public string LastName { get; set; }

        [JsonProperty("password")]
        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(50, ErrorMessage = "Password max size has been exceeded.")]
        public string UserPasswordHash { get; set; }
    }
}
