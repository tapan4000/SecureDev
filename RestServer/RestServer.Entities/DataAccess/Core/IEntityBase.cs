using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.DataAccess.Core
{
    public interface IEntityBase
    {
        string CreatedBy { get; set; }

        DateTime CreationDateTime { get; set; }

        string LastModifiedBy { get; set; }

        DateTime? LastModificationDateTime { get; set; }

        ObjectState ObjectState { get; set; }
    }
}
