using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.BaseModels
{
    public abstract class BusinessRequestData
    {
        protected abstract bool IsValid();
    }
}
