using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class InitiateAddGroupMemberData : BusinessRequestData
    {
        public int GroupId { get; set; }

        public string AddedMemberIsdCode { get; set; }

        public string AddedMemberMobileNumber { get; set; }

        public User RequestorUser { get; set; }

        public bool CanAdminTriggerEmergencySessionForSelf { get; set; }

        public bool CanAdminExtendEmergencySessionForSelf { get; set; }

        public int GroupPeerEmergencyNotificationModePreferenceId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
