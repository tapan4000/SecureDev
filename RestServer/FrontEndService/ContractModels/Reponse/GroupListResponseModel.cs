using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels
{
    using System.Collections;

    using Newtonsoft.Json;

    public class GroupListResponseModel
    {
        [JsonProperty("groups")]
        public IList<GroupModel> Groups { get; set; }
    }
}
