using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace UpdateProjectTableFunctionApp
{
    public static class AddItem
    {

        private static readonly AzureSignalR SignalR = new AzureSignalR(Environment.GetEnvironmentVariable("AzureSignalRConnectionString"));

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo NegotiateConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage request,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Negotiating connection for item: <>.");

                string clientHubUrl = SignalR.GetClientHubUrl("ProjectHub");
                string accessToken = SignalR.GenerateAccessToken(clientHubUrl);
                return new SignalRConnectionInfo { AccessToken = accessToken, Url = clientHubUrl };
            }

            catch (Exception ex)
            {
                log.LogError(ex, "Failed to negotiate connection.");
                throw;
            }
        }


        [FunctionName("AddItem")]
        public static async Task Run([EventHubTrigger("projecteventhub", Connection = "EventHub")] EventData[] events,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    string[] tokens = new string[4];
                    tokens = messageBody.Split('_');

                    string PartitionKey, RowKey, Size, Location;
                    PartitionKey = tokens[0];
                    Size = tokens[1];
                    RowKey = tokens[2];
                    Location = tokens[3];

                    CloudTable table= Utilities.initTable("ProjectTable");
                    // Create an instance of a entity.
                    Entity entity = new Entity(PartitionKey, RowKey, Size, Location);

                    // Check if Location is: sold
                    if (Location == "Sold")
                    {
                        Utilities.DeleteEntity(table, PartitionKey, RowKey);
                    }

                    else
                    {
                        entity = await Utilities.InsertOrMergeEntityAsync(table, entity);

                        // Check if item was added for the first time- if so, need to add item to CounterItems table
                        CloudTable counterItemsTable = Utilities.initTable("CounterItems");
                        ScanEntity isScanned = await Utilities.RetrieveScanEntity(counterItemsTable, "1", PartitionKey);
                        if (isScanned == null) // Need to insert ScanEntity to CounterItems table
                        {
                            ScanEntity scanEntityToAdd = new ScanEntity("1", PartitionKey, 0);
                            await Utilities.InsertOrMergeScanEntity(counterItemsTable, scanEntityToAdd);
                        }
                    }

                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }

                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }


        [FunctionName("check-if-item-exists")]
        public static async Task<Boolean> GetPassword(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "check-if-item-exists/{id}")] HttpRequest req,
         string id,
         ILogger log)
        {
            string[] tokens = new string[2];
            tokens = id.Split('_');

            string PartitionKey, RowKey;
            PartitionKey = tokens[0];
            RowKey = tokens[1];

            CloudTable table = Utilities.initTable("ProjectTable");
            Entity entity = await Utilities.RetrieveEntityUsingPointQueryAsync(table, PartitionKey, RowKey);

            if (entity != null)
            {
                return true;
            }

            else
            {
                return false;
            }
        }


        [FunctionName("search-by-partitionkey")]
        public static List<Entity> GetRowKeys(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "search-by-partitionkey/{Pkey}")] HttpRequest req,
         string Pkey,
         ILogger log)
        {
            CloudTable table = Utilities.initTable("ProjectTable");

            TableQuery<Entity> query = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Pkey));
            List<Entity> entitiesList = new List<Entity>();

            foreach (Entity entity in table.ExecuteQuery(query))
            {
                entitiesList.Add(entity);
            }

            return entitiesList;
        }


        private static async Task<T> ExtractContent<T>(HttpRequestMessage request)
        {
            string connectionRequestJson = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(connectionRequestJson);
        }


    }
}
