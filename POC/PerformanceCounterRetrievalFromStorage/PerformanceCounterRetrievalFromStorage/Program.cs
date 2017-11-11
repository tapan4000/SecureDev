using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCounterRetrievalFromStorage
{
    using System.IO;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {

            ReadMessagesFromStorageTable(
                "DefaultEndpointsProtocol=https;AccountName=feuslogspmadev2;AccountKey=rtaUBFxOH+cLuqlfharoP6g+bvIzUvkDZXr6xr1XnooocwGa6+hi0sWlGPk7KogzwJ4quPKO/JPFolgnazod1Q==",
                "WADPerformanceCountersTable",
                "24/08/2017 1:00:00",
                "24/08/2017 12:00:00",
                "");
        }

        private static void ReadMessagesFromStorageTable(string connectionString, string tableName, string startDate, string endDate, string vin)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference(tableName);
            var startDateFilter = TableQuery.GenerateFilterConditionForDate(
                "EnqueuedTimeUtc",
                QueryComparisons.GreaterThanOrEqual,
                DateTimeOffset.Parse(startDate));
            var endDateFilter = TableQuery.GenerateFilterConditionForDate(
                "EnqueuedTimeUtc",
                QueryComparisons.LessThanOrEqual,
                DateTimeOffset.Parse(endDate));
            var filter = TableQuery.CombineFilters(startDateFilter, TableOperators.And, endDateFilter);
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(filter);

            // Print the fields for each customer.
            foreach (DynamicTableEntity entity in table.ExecuteQuery(query))
            {
                var message = entity.ETag;
                var payload = entity.Properties["Payload"];
                var opCode = entity.Properties["Opcode"].ToString();
                if (opCode.Contains(vin))
                {

                }
            }
        }

        private static void DecryptPayloadPhase1(string payload)
        {
            Convert.FromBase64String(payload);
        }

        private static void DecryptRawPayload(string rawPayload)
        {
            var messageBytes = Convert.FromBase64String(rawPayload);
            var deviceMessageName = string.Empty;
            var messageType = "Alert";
            using (var stream = new MemoryStream(messageBytes))
            {
                ////if (messageType.Equals("Alert"))
                ////{
                ////    var decryptedAlert = TCUAlert.ParseFrom(stream);
                ////    var message = JsonConvert.SerializeObject(decryptedAlert);
                ////}
                ////else if (messageType == "ConnectionStatusAlert")
                ////{
                ////    var decryptedConnectionStatusAlert = TCUConnectionStatusAlert.ParseFrom(stream);
                ////}
                ////else if (messageType == "CommandResponse")
                ////{
                ////    var decryptedCommandResponse = TCUCommandResponse.ParseFrom(stream);
                ////}
                ////else if (messageType == "ModuleSecurityError")
                ////{
                ////    var decryptedModuleSecurityError = ModuleSecurityError.ParseFrom(stream);
                ////}
            }
        }
    }
}
