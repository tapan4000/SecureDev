using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.ServerContext
{
    using System.Runtime.CompilerServices;

    public static class RestServiceContext
    {
        private static bool initialized = false;

        private static string serviceStartupTraceId;
        private static RestServiceHostType restServiceHostType;
        private static string serviceName;
        private static string serviceInstanceName;
        private static string applicationName;
        private static string serviceTypeName;
        private static string dataCenter;
        private static int maxAllowedLogLevel;

        public static string ServiceStartupTraceId
        {
            get
            {
                return ValidateAndGetValue(serviceStartupTraceId);
            }
        }

        public static RestServiceHostType RestServiceHostType
        {
            get
            {
                return ValidateAndGetValue(restServiceHostType);
            }
        }

        public static string ServiceName
        {
            get
            {
                return ValidateAndGetValue(serviceName);
            }
        }

        public static string ServiceInstanceName
        {
            get
            {
                return ValidateAndGetValue(serviceInstanceName);
            }
        }

        public static string ApplicationName
        {
            get
            {
                return ValidateAndGetValue(applicationName);
            }
        }

        public static string ServiceTypeName
        {
            get
            {
                return ValidateAndGetValue(serviceTypeName);
            }
        }

        public static string DataCenter
        {
            get
            {
                return ValidateAndGetValue(dataCenter);
            }
        }

        public static int MaxAllowedLogLevel
        {
            get
            {
                return ValidateAndGetValue(maxAllowedLogLevel);
            }
        }

        public static void InitializeContext(string traceId, RestServiceHostType svcHostType, string serviceIdentifier, string instanceName, string appName, string svcTypeName, string dc, int maxLogLevel)
        {
            if (initialized)
            {
                return;
            }

            serviceStartupTraceId = traceId;
            restServiceHostType = svcHostType;

            if (!string.IsNullOrWhiteSpace(serviceIdentifier) && restServiceHostType == RestServiceHostType.ServiceFabric)
            {
                serviceName = serviceIdentifier.Split('/').Last();
            }

            serviceInstanceName = instanceName;
            applicationName = appName;
            serviceTypeName = svcTypeName;
            dataCenter = dc;
            maxAllowedLogLevel = maxLogLevel;

            initialized = true;
        }

        private static T ValidateAndGetValue<T>(T value)
        {
            if (!initialized)
            {
                throw new ArgumentException($"{nameof(value)} has not been initialized.");
            }

            return value;
        }
    }
}
