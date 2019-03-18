using RestServer.Business.Core.BaseModels;
using RestServer.Business.Models.Response;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Interfaces.Managers
{
    public interface ILocationManager
    {
        Task<AddLocationCaptureSessionResult> AddUserSelfInitiatedLocationCaptureSession(string captureSessionTitle, User RequestingUser,
                LocationCaptureSessionStateEnum locationCaptureSessionState, LocationCaptureTypeEnum locationCaptureType, int groupId, DateTime requestDateTime,
                int locationCapturePeriodInSeconds, string encryptedLatitude, string encryptedLongitude, string encryptedSpeed, string encryptedAltitude);
    }
}
