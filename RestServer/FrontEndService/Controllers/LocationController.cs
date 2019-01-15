using RestServer.Entities.Interfaces;
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
        public async Task<InitiateAddGroupMemberResponseModel> InitiateEmergencySessionForSelf(InitiateAddGroupMemberRequestModel request)
        {
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
            // 6) Store the employee location (per user independent of the emergency session) information in cosmos db. Follow below business rules for cosmos db
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
            
        }
    }
}
