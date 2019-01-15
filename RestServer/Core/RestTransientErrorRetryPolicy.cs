using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Core
{
    public class RestTransientErrorRetryPolicy : TransientErrorRetryPolicyBase
    {
        protected override bool HandleResponse(object response)
        {
            throw new NotImplementedException();
        }

        protected override bool HandleRetryException(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
