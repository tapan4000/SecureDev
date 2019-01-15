using Newtonsoft.Json;
using RestServer.DataAccess.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Models
{
    public class UserLocation : DocumentDbEntityBase
    {
        private int userId;

        public UserLocation() : base(DocumentTypeEnum.UserLocation)
        {
        }

        [JsonProperty("userId")]
        public int UserId
        {
            get
            {
                return this.userId;
            }
            set
            {
                this.userId = value;

                // Set the partition key as the user id and the id as user id prefixed with the location identifier.
                this.PartitionKey = DocumentDbHelper.GetUserDocumentPartitionKey(this.userId);
                this.Id = DocumentDbHelper.GetUserLocationDocumentId(this.userId);
            }
        }

        [JsonProperty("loc")]
        public IList<LocationDetail> LocationDetails;
    }
}
