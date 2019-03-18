using Newtonsoft.Json;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.ContractModels.Request
{
    public class InitiateEmergencySessionForSelfRequestModel
    {
        [JsonProperty("captureTitle")]
        [Required(ErrorMessage = "Emergency session title is required.")]
        public string EmergencySessionTitle { get; set; }

        [JsonProperty("captureState")]
        [Required(ErrorMessage = "Location capture session state is required.")]
        public LocationCaptureSessionStateEnum LocationCaptureSessionState { get; set; }

        [JsonProperty("captureType")]
        [Required(ErrorMessage = "Location capture type is required.")]
        public LocationCaptureTypeEnum LocationCaptureType { get; set; }

        [JsonProperty("groupId")]
        [Required(ErrorMessage = "Group Id is required.")]
        public int GroupId { get; set; }

        [JsonProperty("time")]
        [Required(ErrorMessage = "Request date and time is required.")]
        public DateTime RequestDateTime { get; set; }

        [JsonProperty("captureDuration")]
        [Required(ErrorMessage = "Capture duration is required.")]
        public int LocationCapturePeriodInSeconds { get; set; }

        [JsonProperty("latitude")]
        [Required(ErrorMessage = "Latitude is required.")]
        public string EncryptedLatitude { get; set; }

        [JsonProperty("longitude")]
        [Required(ErrorMessage = "Longitude is required.")]
        public string EncryptedLongitude { get; set; }

        [JsonProperty("speed")]
        public string EncryptedSpeed { get; set; }

        [JsonProperty("altitude")]
        public string EncryptedAltitude { get; set; }
    }
}
