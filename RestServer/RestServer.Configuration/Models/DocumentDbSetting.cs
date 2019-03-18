using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration.Models
{
    public class DocumentDbSetting
    {
        public int MaxConnectionLimit { get; set; }

        public int RequestTimeoutInSeconds { get; set; }

        public int MediaRequestTimeOutInSeconds { get; set; }

        public int RetryCountOnThrottling { get; set; }

        public int RetryIntervalInSeconds { get; set; }

        public string DatabaseName { get; set; }

        public string UserCollectionName { get; set; }
    }
}
