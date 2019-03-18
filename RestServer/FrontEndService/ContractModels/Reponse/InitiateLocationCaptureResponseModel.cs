using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Reponse
{
    public class InitiateLocationCaptureResponseModel : ServiceResponseModel
    {
        [JsonProperty("sessionId")]
        public int ServerLocationCaptureSessionId { get; set; }
    }
}
