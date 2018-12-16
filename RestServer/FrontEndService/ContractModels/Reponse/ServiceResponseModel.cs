using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels
{
    public class ServiceResponseModel
    {
        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
