using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.Controllers
{
    using System.Web.Http;

    using RestServer.FrontEndService.ContractModels;
    using RestServer.Logging.Interfaces;

    [RoutePrefix("api/groups")]
    public class GroupController : ApiController
    {
        private IEventLogger logger;

        public GroupController(IEventLogger logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route]
        public async Task<IList<GroupModel>> GetGroups()
        {
            List<GroupModel> groups = new List<GroupModel>();
            groups.Add(this.GetSampleGroup(1));
            groups.Add(this.GetSampleGroup(2));
            groups.Add(this.GetSampleGroup(3));
            groups.Add(this.GetSampleGroup(4));
            groups.Add(this.GetSampleGroup(5));
            groups.Add(this.GetSampleGroup(6));
            
            this.logger.LogInformation("Group Fetch complete.");
            return await Task.FromResult(groups);
        }

        private GroupModel GetSampleGroup(int groupCounter)
        {
            var groupName = $"Group Member {groupCounter}";

            return new GroupModel()
                       {
                            Name = groupName,
                            Members = new List<GroupMemberModel> {
                                                this.GetSampleGroupMember(groupCounter.ToString(), 1),
                                                this.GetSampleGroupMember(groupCounter.ToString(), 2),
                                                this.GetSampleGroupMember(groupCounter.ToString(), 3),
                                                this.GetSampleGroupMember(groupCounter.ToString(), 4),
                                                this.GetSampleGroupMember(groupCounter.ToString(), 5)},
                            Locations = new List<LocationModel>
                                            {
                                                this.GetSampleLocation(groupCounter.ToString(), 1),
                                                this.GetSampleLocation(groupCounter.ToString(), 2),
                                                this.GetSampleLocation(groupCounter.ToString(), 3),
                                                this.GetSampleLocation(groupCounter.ToString(), 4),
                                                this.GetSampleLocation(groupCounter.ToString(), 5),
                                                this.GetSampleLocation(groupCounter.ToString(), 6)
                                            }
                       };
        }

        private GroupMemberModel GetSampleGroupMember(string groupPrefix, int membercounter)
        {
            var memberName = $"{groupPrefix} Sample Member {membercounter}";
            return new GroupMemberModel { Name = memberName };
        }

        private LocationModel GetSampleLocation(string groupPrefix, int locationCounter)
        {
            var locationName = $"{groupPrefix} Sample Location {locationCounter}";
            return new LocationModel { Name = locationName };
        }
    }
}
