using Newtonsoft.Json;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Models
{
    public abstract class DocumentDbEntityBase : IDocumentDbEntity
    {
        public DocumentDbEntityBase(DocumentTypeEnum documentType)
        {
            this.DocumentType = documentType.ToString();
            this.TimeToLive = -1;
        }

        [JsonProperty("docType")]
        public string DocumentType { get; private set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("ttl")]
        public int TimeToLive { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDateTime { get; set; }

        [JsonProperty("modificationDate")]
        public DateTime? LastModificationDateTime { get; set; }

        [JsonProperty("modifiedBy")]
        public string LastModifiedBy { get; set; }

        [JsonIgnore]
        public ObjectState ObjectState { get; set; }
    }
}
