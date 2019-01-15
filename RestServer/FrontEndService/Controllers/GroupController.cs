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
    using ContractModels.Request;
    using Business.Interfaces.Managers;
    using Entities.Interfaces;
    using Business.Models;
    using ContractModels.Reponse;
    using Filters;

    [RoutePrefix("api/group")]
    [UserAuthenticationFilter]
    public class GroupController : ApiControllerBase
    {
        private IGroupManager groupManager;

        public GroupController(IGroupManager groupManager, IEventLogger logger, IUserContext userContext) : base(logger, userContext)
        {
            this.groupManager = groupManager;
        }

        [HttpPost]
        [Route("initiateAddGroupMember")]
        public async Task<InitiateAddGroupMemberResponseModel> InitiateAddGroupMember(InitiateAddGroupMemberRequestModel request)
        {
            // Usage of API
            // 1) If response status code is 200, all steps were successful. Check the expiry period in days. If the expiry is -1, show message "request sent", else show message "Request sent. The request will expire in x days".
            // 2) Else if the response status code is 400, display message to user "Please check the details provided".
            // 3) Else if the response is Max group count per user reached (701), then display the message "The target user has reached the max group membership threshold, thereby the request cannot be initiate.".
            // 4) Else if the response is Max user count per group reached (702), then display the message "The group has reached it's threshold. Cannot add more members".
            // 5) Else if the response is Add group member denied (703), then display the message "The requested member cannot be added to the target group.".
            // 6) Else if the response is User already added to target group (704), display a message "The user is already a part of the target group.".
            // 7) Else if the response is user already pending acceptance to target group (705), display message that "Request to the member to join target group is already pending".
            // 8) Else display a message indicating, error occurred during adding a member. Please "reattempt adding the member after some time. If the issue continues please open a ticket through ticket center".
            var response = new InitiateAddGroupMemberResponseModel
            {
                Status = (int)PublicStatusCodes.Success,
                ExpiryPeriodInDays = -1
            };

            if (!ModelState.IsValid)
            {
                string errorMessage = string.Format("ModelState Errors: {0}", string.Join(",", ModelState.Values.SelectMany(m => m.Errors).Select(s => s.ErrorMessage)));
                this.logger.LogError(errorMessage);
                response.Status = (int)PublicStatusCodes.BadRequest;
                return response;
            }

            try
            {
                var result = await this.groupManager.InitiateAddGroupMember(
                    request.GroupId, 
                    request.AddedMemberIsdCode, 
                    request.AddedMemberMobileNumber, 
                    this.userContext.User, 
                    request.CanAdminTriggerEmergencySessionForSelf, 
                    request.CanAdminExtendEmergencySessionForSelf, 
                    request.GroupPeerEmergencyNotificationModePreferenceId, 
                    request.IsAdmin);

                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.GroupIdNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserMobileIsdCodeNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserMobileNumberNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserParameterNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.GroupPeerEmergencyNotificationModePreferenceNotProvided))
                    {
                        response.Status = (int)PublicStatusCodes.BadRequest;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.MaxGroupCountPerUserReached))
                    {
                        response.Status = (int)PublicStatusCodes.MaxGroupCountPerUserReached;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.MaxUserCountPerGroupReached))
                    {
                        response.Status = (int)PublicStatusCodes.MaxUserCountPerGroupReached;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.AddGroupMemberDeniedRequestingUserBlocked)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.AddGroupMemberDeniedRequestedGroupBlocked))
                    {
                        response.Status = (int)PublicStatusCodes.AddGroupMemberDenied;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserAlreadyAddedToTargetGroup))
                    {
                        response.Status = (int)PublicStatusCodes.UserAlreadyAddedToTargetGroup;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserAlreadyPendingAcceptanceToGroupMembership))
                    {
                        response.Status = (int)PublicStatusCodes.UserAlreadyPendingAcceptanceToGroupMembership;
                    }
                    else
                    {
                        response.Status = (int)PublicStatusCodes.SystemError;
                    }
                }
                else
                {
                    response.ExpiryPeriodInDays = result.ExpiryPeriodInDays;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogException("Error occurred while initiating request to member to add a group.", ex);
                response.Status = (int)PublicStatusCodes.SystemError;
            }

            return response;
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
