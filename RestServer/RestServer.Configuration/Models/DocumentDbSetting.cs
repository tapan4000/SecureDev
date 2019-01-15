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

        public string ConnectionString { get; set; }

        public int RetryCount { get; set; }

        public int RetryIntervalInSeconds { get; set; }
    }
}
