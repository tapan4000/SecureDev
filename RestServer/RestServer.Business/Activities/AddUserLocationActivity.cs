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
using RestServer.Entities.Enums;
using RestServer.Business.Models;
using RestServer.Entities.DataAccess;
using RestServer.DataAccess.Core.Models;
using RestServer.Core.Extensions;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration;
using RestServer.Configuration.Models;
using RestServer.Business.Core.Interfaces.Activities;

namespace RestServer.Business.Activities
{
    public class AddUserLocationActivity : ActivityBase<AddLocationActivityData, RestrictedBusinessResultBase>
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        private IConfigurationHandler configurationHandler;

        public AddUserLocationActivity(IActivityFactory activityFactory, IEventLogger logger, IUnitOfWorkFactory unitOfWorkFactory, IConfigurationHandler configurationHandler) : base(activityFactory, logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.configurationHandler = configurationHandler;
        }

        protected async override Task<RestrictedBusinessResultBase> ExecuteAsync(AddLocationActivityData requestData)
        {
            var response = new RestrictedBusinessResultBase();
            MembershipTier userMembershipTierConfiguration = null;
            using (var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                if(requestData.LocationGenerationType == LocationGenerationTypeEnum.CaptureSession)
                {
                    // If the user is transmitting the location for a session, check if there is any active capture session in the data store.
                    // As the restrictions on starting a session based on tier is available only at the time of starting a session, there may be fraudulent 
                    // request to update location even though the user may not be having any active sessions.
                    var isActiveCaptureSessionAvailable = await unitOfWork.LocationCaptureSessionRepository.IsActiveCaptureSessionAvailableAccrossGroups(requestData.ContextUser.UserId).ConfigureAwait(false);
                    if (!isActiveCaptureSessionAvailable)
                    {
                        // Check if there is any recently inactivated sessions available.
                        var locationSetting = await this.configurationHandler.GetConfiguration<LocationSetting>(ConfigurationConstants.LocationSetting).ConfigureAwait(false);
                        var isRecentlyInactivatedSessionAvailable = await unitOfWork.LocationCaptureSessionRepository
                                .IsRecentlyInactivatedCaptureSessionAvailableAccrossGroups(requestData.ContextUser.UserId, 
                                    locationSetting.PostInactivationLocationUpdateAllowedPeriodInSeconds).ConfigureAwait(false);

                        if (!isRecentlyInactivatedSessionAvailable)
                        {
                            // If there is no active capture session available, then return with a response indicate no active sessions available.
                            this.Result.IsSuccessful = false;
                            this.Result.AddBusinessError(BusinessErrorCode.NoActiveOrRecentlyInactivatedSessionsAvailable);
                            return response;
                        }
                    }
                }

                userMembershipTierConfiguration = await unitOfWork.MembershipTierRepository.GetById((int)requestData.ContextUser.MembershipTierId).ConfigureAwait(false);
                if(null == userMembershipTierConfiguration)
                {
                    throw new Exception("User membership tier configuration not found.");
                }
            }

            using(var unitOfWork = this.unitOfWorkFactory.DocumentDbUnitOfWork)
            {
                // Fetch user's location document.
                var userLocationDocumentAvailabilityInSeconds = Math.Max(userMembershipTierConfiguration.EmergencySessionAvailabilityInSeconds, userMembershipTierConfiguration.LookoutSessionAvailabilityInSeconds);
                var userLocationLog = await unitOfWork.UserLocationRepository.GetUserLocation(requestData.ContextUser.UserId).ConfigureAwait(false);
                if(null != userLocationLog)
                {
                    // Set the validity period of user's location document to the higher of EmergencySessionAvailability or LookoutSessionAvailability.
                    userLocationLog.TimeToLive = userLocationDocumentAvailabilityInSeconds;

                    // Remove any location entry that is older than user location document availability.
                    if (userLocationLog.LocationDetails.Any())
                    {
                        var currentDateTime = DateTime.UtcNow;
                        int i = 0;
                        int itemsToRemoveCount = 0;
                        while(i < userLocationLog.LocationDetails.Count)
                        {
                            if(userLocationLog.LocationDetails[i].Timestamp.AddSeconds(userLocationDocumentAvailabilityInSeconds) < currentDateTime)
                            {
                                // If the location entry is older than the availability period, then delete the record.
                                itemsToRemoveCount++;
                            }
                            else
                            {
                                // If a record is found that lies within the availability period, no need to check the following records as they would be newer.
                                break;
                            }
                        }
                        
                        for(i = 0; i < itemsToRemoveCount; i++)
                        {
                            userLocationLog.LocationDetails.RemoveAt(0);
                        }
                    }
                    else if (null == userLocationLog.LocationDetails)
                    {
                        userLocationLog.LocationDetails = new List<LocationDetail>();
                    }
                }
                else
                {
                    userLocationLog = new UserLocation
                    {
                        UserId = requestData.ContextUser.UserId,
                        TimeToLive = userLocationDocumentAvailabilityInSeconds,
                        LocationDetails = new List<LocationDetail>()
                    };
                }

                // Add user's location to the document db data store.
                userLocationLog.LocationDetails.Add(new LocationDetail
                {
                    EncryptedLatitude = requestData.EncryptedLatitude,
                    EncryptedLongitude = requestData.EncryptedLongitude,
                    EncryptedSpeed = requestData.EncryptedSpeed,
                    EncryptedAltitude = requestData.EncryptedAltitude,
                    Timestamp = requestData.Timestamp,
                    LocationGenerationType = requestData.LocationGenerationType
                });

                await unitOfWork.UserLocationRepository.InsertOrUpdateUserLocation(userLocationLog).ConfigureAwait(false);
                return response;
            }
        }

        protected override bool ValidateRequestData(AddLocationActivityData requestData)
        {
            if (null == requestData.ContextUser)
            {
                this.logger.LogError("User not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.ContextUserNotProvided);
                return false;
            }

            if (requestData.EncryptedLatitude.IsEmpty())
            {
                this.logger.LogError("Latitude not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LatitudeNotProvided);
                return false;
            }

            if (requestData.EncryptedLongitude.IsEmpty())
            {
                this.logger.LogError("Longitude not provided.");
                this.Result.AddBusinessError(BusinessErrorCode.LongitudeNotProvided);
                return false;
            }

            return true;
        }
    }
}
