using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels
{
    using Newtonsoft.Json;

    public class GroupModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("members")]
        public IList<GroupMemberModel> Members { get; set; }

        [JsonProperty("locations")]
        public IList<LocationModel> Locations { get; set; }
    }
}
