using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FabricShared.BaseServices
{
    public class ServiceFabricLogHelper
    {
        public static string GetServiceRegisteredMessage(int hostProcessId, string serviceType)
        {
            return $"Host Process: {hostProcessId}, Service Type: {serviceType}";
        }

        public static string GetServiceContextAsString(ServiceContext serviceContext)
        {
            return string.Empty;
        }
    }
}
