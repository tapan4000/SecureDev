using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Request
{
    public class InitiateAddGroupMemberRequestModel
    {
        [JsonProperty("groupId")]
        [Required(ErrorMessage = "Group Id is required.")]
        public int GroupId { get; set; }

        [JsonProperty("addedMemberIsdCode")]
        [Required(ErrorMessage = "Added member isd code is required.")]
        public string AddedMemberIsdCode { get; set; }

        [JsonProperty("addedMemberMobileNumber")]
        [Required(ErrorMessage = "Added member mobile number is required.")]
        public string AddedMemberMobileNumber { get; set; }

        [JsonProperty("canAdminTriggerEmergency")]
        [Required(ErrorMessage = "Can admin trigger emergency session flag is required.")]
        public bool CanAdminTriggerEmergencySessionForSelf { get; set; }

        [JsonProperty("canAdminExtendEmergency")]
        [Required(ErrorMessage = "Can admin extend emergency session flag is required.")]
        public bool CanAdminExtendEmergencySessionForSelf { get; set; }

        [JsonProperty("emergencyNotificationPreferenceId")]
        [Required(ErrorMessage = "Emergency notification preference id is required.")]
        public int GroupPeerEmergencyNotificationModePreferenceId { get; set; }

        [JsonProperty("isAdmin")]
        [Required(ErrorMessage = "IsAdmin flag is required.")]
        public bool IsAdmin { get; set; }
    }
}
