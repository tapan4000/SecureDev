using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.IoC
{
    public class ConventionConstants
    {
        public const string InterceptionConvention = "{0}InterceptionBehavior";

        public const string RetryTypeBasedPolicyConvention = "{0}TransientErrorRetryPolicy";
    }
}
