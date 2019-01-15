using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Core
{
    public class RetrySetting
    {
        public RetryStrategyEnum RetryStrategy { get; set; }

        public int RetryCount { get; set; }

        public int RetryIntervalInSeconds { get; set; }

        public string[] TransientExceptionList { get; set; }
    }
}
