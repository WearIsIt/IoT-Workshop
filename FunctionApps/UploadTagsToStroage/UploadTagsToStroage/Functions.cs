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
using System.Net.Http;
using System.Collections.Generic;

namespace UploadTagsToStroage
{
    public static class Functions
    {
        [FunctionName("updateTags")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "updateTags/{itemName}/{tagsString}")] HttpRequestMessage req,
            string itemName, string tagsString,
            ILogger log)
        {

            string[] tagsTokens = tagsString.Split('_');
            CloudTable table = Utilities.initTable();

            foreach (string tag in tagsTokens)
            {
                // Create an instance of a entity.
                Entity entity = await Utilities.RetrieveEntityUsingPointQueryAsync(table, tag, "1");
                Entity newEntity;

                if (entity == null) // tag was not exist
                {
                    newEntity = new Entity(tag, itemName);
                    newEntity = await Utilities.InsertOrMergeEntityAsync(table, newEntity);
                }

                else // tag already exists in tagsTable
                {
                    string itemsOfTag = entity.Items+ "_" +itemName;
                    newEntity = new Entity(tag, itemsOfTag);
                    newEntity = await Utilities.InsertOrMergeEntityAsync(table, newEntity);
                }
            }

            string responseMessage = string.IsNullOrEmpty(itemName)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {itemName}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }


        private static async Task<T> ExtractContent<T>(HttpRequestMessage request)
        {
            string connectionRequestJson = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(connectionRequestJson);
        }



        [FunctionName("getItemsAccordingTags")]
        public static async Task<HashSet<string>> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "getItemsAccordingTags/{tagsString}")] HttpRequestMessage req,
            string tagsString,
            ILogger log)
        {
            string[] tagsTokens = tagsString.Split('_');
            int numOfTags = tagsTokens.Length;
            CloudTable table = Utilities.initTable();
            HashSet<string> allItems = await Utilities.getItemsAsync(tagsTokens, numOfTags, table);
            return allItems;
        }


    }
}
