using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    [Serializable]
    public class ConfigurationChangeMessage
    {
        public ConfigurationChangeMessage()
        {
            this.UpdatedConfigurationKeys = new ConcurrentDictionary<string, Status>();
        }

        public enum Status
        {
            Success,
            Skipped,
            Failed
        }

        public ConcurrentDictionary<string, Status> UpdatedConfigurationKeys { get; set; }
    }
}
