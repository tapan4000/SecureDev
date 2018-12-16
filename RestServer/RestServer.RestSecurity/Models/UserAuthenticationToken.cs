using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestSecurity.Models
{
    public class UserAuthenticationToken
    {
        [JsonProperty("id")]
        public int UserId { get; set; }

        [JsonProperty("aId")]
        public string ApplicationUniqueId { get; set; }

        [JsonProperty("exp")]
        public long ExpiryDateTime { get; set; }

        [JsonProperty("uid")]
        public string UserUniqueId { get; set; }
    }
}
