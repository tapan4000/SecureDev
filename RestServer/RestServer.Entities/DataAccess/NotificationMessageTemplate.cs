using RestServer.Cache.Core.Attributes;
using RestServer.Cache.Core.Enums;
using RestServer.Entities.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.DataAccess
{
    [Serializable]
    [StorageType(CacheHint.LocalCache, CacheArea.Default)]
    public class NotificationMessageTemplate : IEntityBase
    {
        public int NotificationMessageTemplateId { get; set; }

        public int NotificationModeId { get; set; }

        public int NotificationMessageTypeId { get; set; }

        public int LanguageId { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
