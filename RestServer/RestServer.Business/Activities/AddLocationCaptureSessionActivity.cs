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
using RestServer.Core.Extensions;
using RestServer.Business.Models.Response;
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class AddLocationCaptureSessionActivity : ActivityBase<AddLocationCaptureSessionActivityData, AddLocationCaptureSessionResult>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public AddLocationCaptureSessionActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected async override Task<AddLocationCaptureSessionResult> ExecuteAsync(AddLocationCaptureSessionActivityData requestData)
        {
            var response = new AddLocationCaptureSessionResult();

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
                        this.Result.AddBusinessError(BusinessErrorCode.LocationProviderUserIdNotFound);
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
                var userMemberhipTierConfiguration = await unitOfWork.MembershipTierRepository.GetById((int)locationProviderUser.MembershipTierId).ConfigureAwait(false);

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
                    ExpiryDateTime = requestData.RequestDateTime.AddSeconds(requestData.LocationCapturePeriodInSeconds),
                    LocationProviderUserId = requestData.LocationProviderUserId,
                    LocationCaptureSessionStateId = (int)requestData.LocationCaptureSessionStateId,
                    LocationCaptureTypeId = (int)requestData.LocationCaptureTypeId,
                    GroupId = requestData.GroupId,
                    RequestDateTime = requestData.RequestDateTime
                };

                // Store the location capture session to the data store.
                var insertedRecord = await unitOfWork.LocationCaptureSessionRepository.InsertAsync(locationCaptureSession).ConfigureAwait(false);
                await unitOfWork.SaveAsync().ConfigureAwait(false);
                response.ServerLocationCaptureSessionId = insertedRecord.LocationCaptureSessionId;
            }

            return response;
        }

        protected override bool ValidateRequestData(AddLocationCaptureSessionActivityData requestData)
        {
            if(null == requestData.RequestingUser)
            {
                this.logger.LogError("Requesting user is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserParameterNotProvided);
                return false;
            }

            if (requestData.Title.IsEmpty())
            {
                this.logger.LogError("Location capture session title is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LocationCaptureSessionTitleNotProvided);
                return false;
            }

            if (requestData.LocationProviderUserId <= 0)
            {
                this.logger.LogError("User id for location provider user is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.UserIdNotProvided);
                return false;
            }

            if (requestData.GroupId <= 0)
            {
                this.logger.LogError("Group id for location capture session is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.GroupIdNotProvided);
                return false;
            }

            if (requestData.LocationCapturePeriodInSeconds <= 0)
            {
                this.logger.LogError("Duration for location capture session is not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LocationCaptureSessionDurationNotProvided);
                return false;
            }

            return true;
        }
    }
}
