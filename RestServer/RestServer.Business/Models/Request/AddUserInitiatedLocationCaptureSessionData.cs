﻿using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class AddUserInitiatedLocationCaptureSessionData : BusinessRequestData
    {
        public string CaptureSessionTitle { get; set; }

        public int LocationProviderUserId { get; set; }

        public LocationCaptureSessionStateEnum LocationCaptureSessionStateId { get; set; }

        public LocationCaptureTypeEnum LocationCaptureTypeId { get; set; }

        public int GroupId { get; set; }

        public DateTime RequestDateTime { get; set; }

        public int LocationCapturePeriodInSeconds { get; set; }

        public User RequestingUser { get; set; }

        public string EncryptedLatitude;

        public string EncryptedLongitude;

        public string EncryptedSpeed;

        public string EncryptedAltitude;
    }
}