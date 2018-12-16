using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestSecurity.Models
{
    public class SignedUserAuthenticationToken
    {
        [JsonProperty("token")]
        public UserAuthenticationToken Token { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
