using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Models
{
    public class LocationDetail
    {
        [JsonProperty("lat")]
        public string EncryptedLatitude;

        [JsonProperty("lon")]
        public string EncryptedLongitude;

        [JsonProperty("spd")]
        public string EncryptedSpeed;

        [JsonProperty("alt")]
        public string EncryptedAltitude;

        [JsonProperty("time")]
        public DateTime Timestamp;

        public LocationRequestType(Adhoc/Periodic) RequestType;
    }
}
