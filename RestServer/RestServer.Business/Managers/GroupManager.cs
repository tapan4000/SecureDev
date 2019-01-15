using RestServer.Business.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using RestServer.Business.Core.Interfaces.Processors;
using RestServer.Business.Processors;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;

namespace RestServer.Business.Managers
{
    public class GroupManager : IGroupManager
    {
        private IProcessorFactory businessProcessorFactory;
        public GroupManager(IProcessorFactory businessProcessorFactory)
        {
            this.businessProcessorFactory = businessProcessorFactory;
        }

        public async Task<AddGroupMemberProcessorResult> InitiateAddGroupMember(int groupId, string addedMemberIsdCode, string addedMemberMobileNumber, User requestorUser, bool canAdminTriggerEmergencySessionForSelf, bool canAdminExtendEmergencySessionForSelf, int groupPeerEmergencyNotificationModePreferenceId, bool isAdmin)
        {
            var initiateAddGroupMemberProcessor = this.businessProcessorFactory.CreateProcessor<InitiateAddGroupMemberProcessor, InitiateAddGroupMemberData, AddGroupMemberProcessorResult>();
            var initiateAddGroupMemberRequest = new InitiateAddGroupMemberData
            {
                GroupId = groupId,
                AddedMemberIsdCode = addedMemberIsdCode,
                AddedMemberMobileNumber = addedMemberMobileNumber,
                RequestorUser = requestorUser,
                CanAdminTriggerEmergencySessionForSelf = canAdminTriggerEmergencySessionForSelf,
                CanAdminExtendEmergencySessionForSelf = canAdminExtendEmergencySessionForSelf,
                GroupPeerEmergencyNotificationModePreferenceId = groupPeerEmergencyNotificationModePreferenceId,
                IsAdmin = isAdmin
            };

            return await initiateAddGroupMemberProcessor.TrackAndExecuteAsync(initiateAddGroupMemberRequest);
        }
    }
}
