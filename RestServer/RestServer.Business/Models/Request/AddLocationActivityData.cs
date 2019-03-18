using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class AddLocationActivityData : BusinessRequestData
    {
        public User ContextUser { get; set; }

        public string EncryptedLatitude;

        public string EncryptedLongitude;

        public string EncryptedSpeed;

        public string EncryptedAltitude;

        public DateTime Timestamp;

        public LocationGenerationTypeEnum LocationGenerationType;
    }
}
