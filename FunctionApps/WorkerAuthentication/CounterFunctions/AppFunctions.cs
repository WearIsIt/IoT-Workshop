using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace ProjectFunctions
{
    public static class AppFunctions
    {
        
        [FunctionName("get-password")]
        public static async Task<Entity> GetPassword(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "get-password/{username}")] HttpRequest req,
            string username,
            ILogger log)      
        {
            // Get the user from the table
            BasicSamples basicSamples = new BasicSamples();
            Entity entity = await basicSamples.RunSamples(username);

            // return a response to the user
            if (entity!=null)
            {
                entity.Password = Cryptography.Decrypt(entity.Password);
            }

            return entity;
        }


        private static async Task<T> ExtractContent<T>(HttpRequestMessage request)
        {
            string connectionRequestJson = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(connectionRequestJson);
        }
  

    }
}
