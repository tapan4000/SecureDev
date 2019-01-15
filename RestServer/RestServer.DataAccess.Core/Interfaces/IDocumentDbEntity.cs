using RestServer.Entities.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces
{
    public interface IDocumentDbEntity : IEntityBase
    {
        string Id { get; set; }

        string PartitionKey { get; set; }

        int TimeToLive { get; set; }
    }
}
