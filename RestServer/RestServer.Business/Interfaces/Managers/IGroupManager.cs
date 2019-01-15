using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Response;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Interfaces.Managers
{
    public interface IGroupManager
    {
        Task<AddGroupMemberProcessorResult> InitiateAddGroupMember(int groupId, string addedMemberIsdCode, string addedMemberMobileNumber, User requestorUser, bool canAdminTriggerEmergencySessionForSelf, bool canAdminExtendEmergencySessionForSelf, int groupPeerEmergencyNotificationModePreferenceId, bool isAdmin);
    }
}
