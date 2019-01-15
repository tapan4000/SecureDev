using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration.Models
{
    public class GlobalSetting
    {
        public int MinIocpThreadCountForMaxRedisThroughput { get; set; }

        public int SqlRetryCount { get; set; }

        public int SqlRetryIntervalInSeconds { get; set; }

        public int SqlCommandTimeoutInSeconds { get; set; }
    }
}
