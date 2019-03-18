using RestServer.Business.Interfaces.Managers;
using RestServer.Business.Models;
using RestServer.Entities.Interfaces;
using RestServer.FrontEndService.ContractModels;
using RestServer.FrontEndService.ContractModels.Reponse;
using RestServer.FrontEndService.ContractModels.Request;
using RestServer.FrontEndService.Filters;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestServer.FrontEndService.Controllers
{
    [RoutePrefix("api/location")]
    [UserAuthenticationFilter]
    public class LocationController : ApiControllerBase
    {
        private ILocationManager locationManager;

        public LocationController(ILocationManager locationManager, IEventLogger logger, IUserContext userContext) : base(logger, userContext)
        {
            this.locationManager = locationManager;
        }

        [HttpPost]
        [Route("initiateEmergency")]
        public async Task<InitiateLocationCaptureResponseModel> InitiateEmergencySessionForSelf(InitiateEmergencySessionForSelfRequestModel request)
        {
            // Usage of API
            // 1) If the response code is 200, update the server side emergency session id on the client.
            // 2) If the response code is group has no other admin (706), then the request is succeeded, however, user is shown a message indicating no other admin to adddress the emergency.
            // 2) If the response code is not 200, check if the response contains a server side emergency session id. If yes, update the id on the client and process the status code.
            // 2) Else if the response status code is 400, display message to user "Please check the details provided".
            // 3) Not applicable to this case: Else if the response is Location provider user id not found (801), then display the message "The target user is not part of the group.".
            // 4) Else if the response is user requesting location not associated to target group (802), then display the message "You are not associated to the group. Cannot start the emergency session.".
            // 5) Else if the response is location provider user not associated to target group (803), then display the message "The requested member is not associated to the target group.".
            // 6) Not applicable to this case: Else if the response is user requesting location not an admin (804), display a message "You need to be an admin to trigger the location request.".
            // 7) Else if the response is location capture session exceeded allowed duration (805), display message that "Location capture session cannot be initiated as the duration is more than the allowed limit.".
            // 8) Not applicable to this scenario: Else if the response is no active or recently inactivated session available (806), display message that "No active session found".
            // 9) Else display a message indicating, error occurred during starting the session. Please contact support.".

            // User triggers the emergency for self and below is the workflow
            // 1) Local entry is created in the mobile database with Local emergency id.
            // 2) Send the request to server in order to save the emergency session and get the server emergency session id.
            // 3) Each time a location updated happens on mobile, check if there exists any location data for that session in the local database. If yes, then store
            //    the new record in the local database and attempt to send the records in batches, clearing the batch from local db once the call is complete.
            //    Else if there exists no record in the local db, then directly attempt to send the location update to the server. If the send fails, then store the 
            //    location information in the local database.
            // 4) A background process should check periodically (once a day) to determine if there are any emergency sessions that are inactive and still pending to
            //    be transmitted to the server.
            // 5) User can either stop the session, delete the session or extend the session. If the user stops the session follow the same process of notifying the server
            //    as is used while transmitting location to server. If the user extends the session the extension related information needs to be stored in a separate
            //    table (SessionExtensionHistory) for determine who extended the session and for how long.
            // 6) Store the user location (per user independent of the emergency session) information in cosmos db. Follow below business rules for cosmos db
            //    i) Extend TTL of record by the validity period (14 days) on each new location insert into the db. This can be done by first checking the first record of the 
            //       to see if it is older than 14 days.
            //    ii) On any fetch of emergency session information from cosmos db, iterate over the records and compare with start and end time of the emergency session,
            //        and return only those records that lie between the start and end time.
            //    iii) On stop session update SQL database with the Status as stopped and if no other active emergency sessions are present ask the mobile app to stop location
            //         updates.
            //    iv) The Emergency location table needs to be cleared of the expired records every month to get rid of unnecessary records. Additionally, the defregmentation
            //        of table needs to be done post cleanup.
            //    v) The columns of the cosmos db can be: Encrypted location, latitude, speed (configurable), altitude (configurable), time.
            // 7) As admins from multiple groups can trigger emergency for same user, the user needs to set the emergency update frequency that will be same across all the 
            //    groups.
            // 8) If some admins have requested for emergency in one group for User A and other admins in another group have requested for perdiodic updates for User A,
            //    the both the groups will get the location update at same frequency and the frequency will be faster of the emergency or periodic update frequency.
            // 9) Create tables for AdhocLoationRequest and AdhocLocationResponse and LocationRequestSession. The location information from adhoc requests should be stored
            //    in both the AdhocLocationResponse table and the UserLocation document.
            var response = new InitiateLocationCaptureResponseModel
            {
                Status = (int)PublicStatusCodes.Success,
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
                var result = await this.locationManager.AddUserSelfInitiatedLocationCaptureSession(
                    request.EmergencySessionTitle,
                    this.userContext.User,
                    request.LocationCaptureSessionState,
                    request.LocationCaptureType,
                    request.GroupId,
                    request.RequestDateTime,
                    request.LocationCapturePeriodInSeconds,
                    request.EncryptedLatitude,
                    request.EncryptedLongitude,
                    request.EncryptedSpeed,
                    request.EncryptedAltitude);

                response.ServerLocationCaptureSessionId = result.ServerLocationCaptureSessionId;

                if (!result.IsSuccessful)
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LocationCaptureSessionTitleNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserIdNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.GroupIdNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserParameterNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LocationCaptureSessionDurationNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LatitudeNotProvided)
                        || result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LongitudeNotProvided))
                    {
                        response.Status = (int)PublicStatusCodes.BadRequest;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LocationProviderUserIdNotFound))
                    {
                        response.Status = (int)PublicStatusCodes.LocationProviderUserIdNotFound;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserRequestingLocationNotAssociatedToTargetGroup))
                    {
                        response.Status = (int)PublicStatusCodes.UserRequestingLocationNotAssociatedToTargetGroup;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LocationProviderUserNotAssociatedToTargetGroup))
                    {
                        response.Status = (int)PublicStatusCodes.LocationProviderUserNotAssociatedToTargetGroup;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.UserRequestingLocationNotAnAdmin))
                    {
                        response.Status = (int)PublicStatusCodes.UserRequestingLocationNotAnAdmin;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.LocationCaptureSessionExceededAllowedDuration))
                    {
                        response.Status = (int)PublicStatusCodes.LocationCaptureSessionExceededAllowedDuration;
                    }
                    else if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.NoActiveOrRecentlyInactivatedSessionsAvailable))
                    {
                        response.Status = (int)PublicStatusCodes.NoActiveOrRecentlyInactivatedSessionsAvailable;
                    }
                    else
                    {
                        response.Status = (int)PublicStatusCodes.SystemError;
                    }
                }
                else
                {
                    if (result.BusinessErrors.Any(error => error.ErrorCode == BusinessErrorCode.GroupHasNoAdminOtherThanRequestingUser))
                    {
                        response.Status = (int)PublicStatusCodes.GroupHasNoAdminOtherThanRequestingUser;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogException("Error occurred while initiating emergency session for self.", ex);
                response.Status = (int)PublicStatusCodes.SystemError;
            }

            return response;
        }
    }
}
