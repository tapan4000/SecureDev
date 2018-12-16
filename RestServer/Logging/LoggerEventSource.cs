namespace RestServer.Logging
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Security;

    using RestServer.Logging.Interfaces;
    using RestServer.ServerContext;
    using RestServer.Utility;
    using Microsoft.Diagnostics.Tracing;

    [EventSource(Name="RestServer-EventLog")]
    public sealed class LoggerEventSource : LoggerEventSourceBase
    {
        private const int InformationEventId = 1;
        private const int VerboseEventId = 2;
        private const int ErrorEventId = 3;
        private const int WarningEventId = 4;
        private const int CriticalEventId = 5;
        private const int ExecutionTimeEventId = 6;

        private const string NullString = "";

        private static readonly Lazy<LoggerEventSource> Instance =
            new Lazy<LoggerEventSource>(() => new LoggerEventSource());

        public static LoggerEventSource Current => Instance.Value;

        [Event(VerboseEventId, Level = EventLevel.Verbose, Message = "{3}", Channel = EventChannel.Operational)]
        protected override void VerboseStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime)
        {
            if (IsLogLevelAllowed(EventLevel.Verbose))
            {
                this.WriteStandardEventMessage(
                VerboseEventId,
                traceId,
                serviceName,
                instanceName,
                message,
                errorMessage,
                userId,
                logType,
                dataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
            }
        }

        [Event(InformationEventId, Level = EventLevel.Informational, Message = "{3}", Channel = EventChannel.Operational)]
        protected override void InfoStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime)
        {
            if (IsLogLevelAllowed(EventLevel.Informational))
            {
                this.WriteStandardEventMessage(
                InformationEventId,
                traceId,
                serviceName,
                instanceName,
                message,
                errorMessage,
                userId,
                logType,
                dataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
            }
        }

        [Event(WarningEventId, Level = EventLevel.Warning, Message = "{3}", Channel = EventChannel.Operational)]
        protected override void WarningStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime)
        {
            if (IsLogLevelAllowed(EventLevel.Warning))
            {
                this.WriteStandardEventMessage(
                WarningEventId,
                traceId,
                serviceName,
                instanceName,
                message,
                errorMessage,
                userId,
                logType,
                dataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
            }
        }

        [Event(ErrorEventId, Level = EventLevel.Error, Message = "{3}", Channel = EventChannel.Operational)]
        protected override void ErrorStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime)
        {
            if (IsLogLevelAllowed(EventLevel.Error))
            {
                this.WriteStandardEventMessage(
                ErrorEventId,
                traceId,
                serviceName,
                instanceName,
                message,
                errorMessage,
                userId,
                logType,
                dataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
            }
        }

        [Event(CriticalEventId, Level = EventLevel.Critical, Message = "{3}", Channel = EventChannel.Operational)]
        protected override void CriticalStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime)
        {
            if (IsLogLevelAllowed(EventLevel.Critical))
            {
                this.WriteStandardEventMessage(
                CriticalEventId,
                traceId,
                serviceName,
                instanceName,
                message,
                errorMessage,
                userId,
                logType,
                dataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
            }
        }

        private static bool IsLogLevelAllowed(EventLevel logLevel)
        {
            return RestServiceContext.MaxAllowedLogLevel >= (int)logLevel;
        }

        private void WriteMessageToSql(string message)
        {
            using (
                SqlConnection connection =
                    new SqlConnection(
                        "Server=tcp:cmpserver1.database.windows.net,1433;Initial Catalog=cmp;Persist Security Info=False;User ID=cmpadmin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                )
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"Insert into [Logs] values('{message}')");
                command.Connection = connection;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private unsafe void WriteStandardEventMessage(int eventId, string traceId, string serviceName, string serviceInstanceName, string message, string errorMessage, int userId, 
            string logType, string dataCenter, string memberName, string fileName, int lineNumber, string logTime)
        {
            const int ArgumentCount = 12;

            if (string.IsNullOrWhiteSpace(logTime))
            {
                logTime = LogHelper.CurrentFormattedDateTime;
            }

            if (traceId == null)
            {
                traceId = NullString;
            }

            if (serviceName == null)
            {
                serviceName = NullString;
            }

            if (message == null)
            {
                message = NullString;
            }

            if (errorMessage == null)
            {
                errorMessage = NullString;
            }

            if (logType == null)
            {
                logType = NullString;
            }

            if (dataCenter == null)
            {
                dataCenter = NullString;
            }

            if (memberName == null)
            {
                memberName = NullString;
            }

            if(serviceInstanceName == null)
            {
                serviceInstanceName = NullString;
            }

            if (fileName == null)
            {
                fileName = NullString;
            }
            else
            {
                fileName = Utility.GetClassName(fileName);
            }
            

            fixed(
                char* dataTraceId = traceId,
                dataServiceName = serviceName,
                dataServiceInstanceName = serviceInstanceName,
                dataMessage = message,
                dataErrorMessage = errorMessage,
                dataLogType = logType,
                dataDatacenter = dataCenter,
                dataMemberName = memberName,
                dataFileName = fileName,
                logtime = logTime)
            {
                EventData* eventData = stackalloc EventData[ArgumentCount];

                eventData[0] = new EventData { DataPointer = (IntPtr)dataTraceId, Size = this.SizeInBytes(traceId) };
                eventData[1] = new EventData { DataPointer = (IntPtr)dataServiceName, Size = this.SizeInBytes(serviceName) };
                eventData[2] = new EventData { DataPointer = (IntPtr)dataServiceInstanceName, Size = this.SizeInBytes(serviceInstanceName) };
                eventData[3] = new EventData { DataPointer = (IntPtr)dataMessage, Size = this.SizeInBytes(message) };
                eventData[4] = new EventData { DataPointer = (IntPtr)dataErrorMessage, Size = this.SizeInBytes(errorMessage) };
                eventData[5] = new EventData { DataPointer = (IntPtr)(&userId), Size = sizeof(int) };
                eventData[6] = new EventData { DataPointer = (IntPtr)dataLogType, Size = this.SizeInBytes(logType) };
                eventData[7] = new EventData { DataPointer = (IntPtr)dataDatacenter, Size = this.SizeInBytes(dataCenter) };
                eventData[8] = new EventData { DataPointer = (IntPtr)dataMemberName, Size = this.SizeInBytes(memberName) };
                eventData[9] = new EventData { DataPointer = (IntPtr)dataFileName, Size = this.SizeInBytes(fileName) };
                eventData[10] = new EventData { DataPointer = (IntPtr)(&lineNumber), Size = sizeof(int) };
                eventData[11] = new EventData { DataPointer = (IntPtr)logtime, Size = this.SizeInBytes(logTime) };

                this.WriteEventCore(eventId, ArgumentCount, eventData);
            }
        }

        private int SizeInBytes(string value)
        {
            return null == value ? 0 : (value.Length + 1) * sizeof(char);
        }
    }
}
