using RestServer.Business.Core.Activities;
using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.Business.Models;
using RestServer.Entities.Enums;

namespace RestServer.Business.Activities
{
    public class AddLocationCaptureSessionActivity : ActivityBase<AddLocationCaptureSessionActivityData, RestrictedBusinessResultBase>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public AddLocationCaptureSessionActivity(IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<RestrictedBusinessResultBase> ExecuteAsync(AddLocationCaptureSessionActivityData requestData)
        {
            var response = new RestrictedBusinessResultBase();

            // Add the user location capture session header information in data store
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                User locationProviderUser;
                if(requestData.RequestingUser.UserId != requestData.LocationProviderUserId)
                {
                    // If the user is not initiating the location capture session for self, then fetch the user for whom the location capture session is
                    // to be initiated to determine the membership tier of the user.
                    locationProviderUser = await unitOfWork.UserRepository.GetById(requestData.LocationProviderUserId).ConfigureAwait(false);
                    if(null == locationProviderUser)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.UserWithIdNotFound);
                        return response;
                    }
                }
                else
                {
                    locationProviderUser = requestData.RequestingUser;
                }

                // Verify that the user requesting the location and the user whose location is requested is part of the group. Additionally,
                // if the a user is requesting location for another user, then the requesting user should be an admin.
                var requestingUserGroupMemberRecord = await unitOfWork.GroupMemberRepository.GetExistingGroupMemberRecord(requestData.GroupId, requestData.RequestingUser.UserId).ConfigureAwait(false);
                if (null == requestingUserGroupMemberRecord)
                {
                    this.Result.IsSuccessful = false;
                    this.Result.AddBusinessError(BusinessErrorCode.UserRequestingLocationNotAssociatedToTargetGroup);
                    return response;
                }

                if (requestData.RequestingUser.UserId != requestData.LocationProviderUserId)
                {
                    // If the user is requesting location for another user, check if the user providing location is part of the group.
                    var isLocationProviderUserPartOfTheGroup = await unitOfWork.GroupMemberRepository.IsUserAlreadyAddedToGroup(requestData.GroupId, requestData.LocationProviderUserId).ConfigureAwait(false);
                    if (!isLocationProviderUserPartOfTheGroup)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.LocationProviderUserNotAssociatedToTargetGroup);
                        return response;
                    }

                    // Verify if the user requesting the location is an admin.
                    if (!requestingUserGroupMemberRecord.IsAdmin)
                    {
                        this.Result.IsSuccessful = false;
                        this.Result.AddBusinessError(BusinessErrorCode.UserRequestingLocationNotAnAdmin);
                        return response;
                    }
                }

                // Fetch the membership tier of the user whose location is requested to determine if the requested duration exceeds the max allowed duration.
                var userMemberhipTierConfiguration = await unitOfWork.MembershipTierRepository.GetById(locationProviderUser.MembershipTierId).ConfigureAwait(false);

                if((requestData.LocationCaptureTypeId == LocationCaptureTypeEnum.Emergency && requestData.LocationCapturePeriodInSeconds > userMemberhipTierConfiguration.EmergencySessionMaxDurationInSeconds) 
                    || (requestData.LocationCaptureTypeId == LocationCaptureTypeEnum.PeriodicUpdate && requestData.LocationCapturePeriodInSeconds > userMemberhipTierConfiguration.LookoutSessionMaxDurationInSeconds))
                {
                    this.Result.IsSuccessful = false;
                    this.Result.AddBusinessError(BusinessErrorCode.LocationCaptureSessionExceededAllowedDuration);
                    return response;
                }

                var locationCaptureSession = new LocationCaptureSession
                {
                    Title = requestData.Title,
                    ExpiryDateTime = DateTime.UtcNow.AddSeconds(requestData.LocationCapturePeriodInSeconds),
                    LocationProviderUserId = requestData.LocationProviderUserId,
                    LocationCaptureSessionStateId = (int)requestData.LocationCaptureSessionStateId,
                    LocationCaptureTypeId = (int)requestData.LocationCaptureTypeId,
                    GroupId = requestData.GroupId,
                    RequestDateTime = requestData.RequestDateTime
                };

                // Store the location capture session to the data store.
                var insertedRecord = await unitOfWork.LocationCaptureSessionRepository.InsertAsync(locationCaptureSession).ConfigureAwait(false);
                await unitOfWork.SaveAsync().ConfigureAwait(false);
            }

            return response;
        }

        protected override bool ValidateRequestData(AddLocationCaptureSessionActivityData requestData)
        {
            throw new NotImplementedException();
        }
    }
}
