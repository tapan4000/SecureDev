using Newtonsoft.Json;
using RestServer.DataAccess.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Models
{
    public class UserBlockList : DocumentDbEntityBase
    {
        private int userId;

        public UserBlockList() : base(DocumentTypeEnum.UserBlockList)
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

                // Set the partition key and id as the user id.
                this.PartitionKey = DocumentDbHelper.GetUserDocumentPartitionKey(this.userId);
                this.Id = DocumentDbHelper.GetUserBlockListDocumentId(this.userId);
            }
        }

        [JsonProperty("blockedUserIds")]
        public IList<int> BlockedUserIds;

        [JsonProperty("blockedGroupIds")]
        public IList<int> BlockedGroupIds;
    }
}
