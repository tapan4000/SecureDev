using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Reponse
{
    public class InitiateAddGroupMemberResponseModel : ServiceResponseModel
    {
        [JsonProperty("expiryInDays")]
        public int ExpiryPeriodInDays { get; set; }
    }
}
