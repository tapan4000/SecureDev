using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Models
{
    public class DocumentDbRecord<TEntity>
    {
        public TEntity Entity { get; set; }

        public DateTime LastModificationDateTime { get; set; }

        public int? TimeToLive { get; set; }
    }
}
