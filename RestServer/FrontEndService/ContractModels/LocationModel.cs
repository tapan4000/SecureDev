using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels
{
    using Newtonsoft.Json;

    public class LocationModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
