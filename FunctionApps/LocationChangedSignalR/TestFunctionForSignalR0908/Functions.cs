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

namespace TestFunctionForSignalR0908
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

                string clientHubUrl = SignalR.GetClientHubUrl("LocationHub");
                string accessToken = SignalR.GenerateAccessToken(clientHubUrl);
                return new SignalRConnectionInfo { AccessToken = accessToken, Url = clientHubUrl };
            }

            catch (Exception ex)
            {
                log.LogError(ex, "Failed to negotiate connection.");
                throw;
            }
        }


        [FunctionName("location-changed-signalR")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "location-changed-signalR/{itemName}")] HttpRequest req,
            [SignalR(HubName = "LocationHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            string itemName,
            ILogger log)
        {

            await callSignalRAsync(signalRMessages, itemName);

        }


        public static async Task callSignalRAsync(IAsyncCollector<SignalRMessage> signalRMessages, string itemName)
        {
            await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "LocationUpdate",
                        Arguments = new object[] { itemName }
                    });
        }

    }
}
