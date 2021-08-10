using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net.Http;

namespace ScanCountFunctionApp
{
    public static class Functions
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

                string clientHubUrl = SignalR.GetClientHubUrl("ScanHub");
                string accessToken = SignalR.GenerateAccessToken(clientHubUrl);
                return new SignalRConnectionInfo { AccessToken = accessToken, Url = clientHubUrl };
            }

            catch (Exception ex)
            {
                log.LogError(ex, "Failed to negotiate connection.");
                throw;
            }
        }


        [FunctionName("update-item-counter")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "update-item-counter/{itemName}")] HttpRequest req,
            [SignalR(HubName = "ScanHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            string itemName,
            ILogger log)
        {
            CloudTable table = Utilities.initTable("CounterItems");
            // Retrieve instance of a entity.
            Entity UpdatedEntity = await Utilities.RetrieveEntityUsingPointQueryAsync(table, "1", itemName);

            // Increase entity's counter by 1
            UpdatedEntity.ScanCounter++;

            // Update updatedEntity to storage
            UpdatedEntity = await Utilities.InsertOrMergeEntityAsync(table, UpdatedEntity);

            List<Entity> topFiveList= getTopFiveItems();

            // Check if UpdatedEntity is already in Top Five
            foreach (Entity currTopFiveEntity in topFiveList)
            {
                if (currTopFiveEntity.RowKey== UpdatedEntity.RowKey)
                {
                    currTopFiveEntity.ScanCounter++;
                    await Utilities.InsertOrMergeEntityAsync(table, currTopFiveEntity);

                    // Call SignalR
                    await callSignalRAsync(signalRMessages, itemName);

                    return;
                }               
            }
          
            foreach (Entity currTopFiveEntity in topFiveList)
            {
                if (currTopFiveEntity.ScanCounter < UpdatedEntity.ScanCounter)
                {
                    Entity entityToAdd = new Entity("2", UpdatedEntity.RowKey, UpdatedEntity.ScanCounter);

                    // Add UpdatedEntity to top five instead of currTopFiveEntity
                    await Utilities.SwapEntitiesAsync(table, currTopFiveEntity, entityToAdd);

                    // Call SignalR
                    await callSignalRAsync(signalRMessages, itemName);

                    break;
                }
            }
        }


        public static async Task callSignalRAsync(IAsyncCollector<SignalRMessage> signalRMessages, string itemName)
        {
            await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "ScanUpdate",
                        Arguments = new object[] { itemName }
                    });
        }


        [FunctionName("retrieve-top-five")]
        public static TopEntity[] Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "retrieve-top-five")] HttpRequest req,
            ILogger log)
        {
            List<Entity> topFiveListOfEntities =getTopFiveItems();
            topFiveListOfEntities.Sort((x, y) => y.ScanCounter.CompareTo(x.ScanCounter));

            TopEntity[] topFiveListOfTopEntities = new TopEntity[5];
            for (int i=0;i< 5; i++)
            {
                TopEntity currTopEntity = new TopEntity(topFiveListOfEntities[i].RowKey, i+1, 
                    topFiveListOfEntities[i].ScanCounter);

                topFiveListOfTopEntities[i] = currTopEntity;
            }

            return topFiveListOfTopEntities;
        }


        private static List<Entity> getTopFiveItems()
        {
            CloudTable table = Utilities.initTable("CounterItems");

            TableQuery<Entity> query = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "2"));
            List<Entity> entitiesList = new List<Entity>();

            foreach (Entity entity in table.ExecuteQuery(query))
            {
                entitiesList.Add(entity);
            }

           return entitiesList;
        }


    }
}
