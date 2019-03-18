using RestServer.Business.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Enums;
using RestServer.Business.Core.Interfaces.Processors;
using RestServer.Business.Processors;
using RestServer.Business.Models.Request;
using RestServer.Business.Models.Response;

namespace RestServer.Business.Managers
{
    public class LocationManager : ILocationManager
    {
        private IProcessorFactory businessProcessorFactory;

        public LocationManager(IProcessorFactory businessProcessorFactory)
        {
            this.businessProcessorFactory = businessProcessorFactory;
        }

        public async Task<AddLocationCaptureSessionResult> AddUserSelfInitiatedLocationCaptureSession(string captureSessionTitle, User requestingUser, 
            LocationCaptureSessionStateEnum locationCaptureSessionStateId, LocationCaptureTypeEnum locationCaptureTypeId, int groupId, DateTime requestDateTime, 
            int locationCapturePeriodInSeconds, string encryptedLatitude, string encryptedLongitude, string encryptedSpeed, string encryptedAltitude)
        {
            var addUserSelfInitiatedLocationCaptureSessionProcessor = this.businessProcessorFactory.CreateProcessor<AddUserSelfInitiatedLocationCaptureSessionProcessor, AddUserInitiatedLocationCaptureSessionData, AddLocationCaptureSessionResult>();
            var initiateAddGroupMemberRequest = new AddUserInitiatedLocationCaptureSessionData
            {
                CaptureSessionTitle = captureSessionTitle,
                LocationProviderUserId = requestingUser.UserId,
                LocationCaptureSessionStateId = locationCaptureSessionStateId,
                LocationCaptureTypeId = locationCaptureTypeId,
                GroupId = groupId,
                RequestDateTime = requestDateTime,
                LocationCapturePeriodInSeconds = locationCapturePeriodInSeconds,
                RequestingUser = requestingUser,
                EncryptedLatitude = encryptedLatitude,
                EncryptedLongitude = encryptedLongitude,
                EncryptedSpeed = encryptedSpeed,
                EncryptedAltitude = encryptedAltitude
            };

            return await addUserSelfInitiatedLocationCaptureSessionProcessor.TrackAndExecuteAsync(initiateAddGroupMemberRequest);
        }
    }
}
