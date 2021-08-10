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

namespace CustomerLogIn
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

            // Return a response to the user
            if (entity != null)
            {
                // Decrypt password
                entity.Password = Cryptography.Decrypt(entity.Password);
            }
            return entity;
        }


        [FunctionName("signup")]
        public static async Task<Entity> SignUp(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
             ILogger log)
        {
            Entity entity = await ExtractContent<Entity>(req);

            // Ecrypt password
            string encryptedPassword = Cryptography.Encrypt(entity.Password);
            entity.Password = encryptedPassword;
            entity.Favorites = "NoItems";

            BasicSamples basicSamples = new BasicSamples();
            Entity newEntity= await basicSamples.RunSamplesForSignUp(entity);
            
            return newEntity;
        }


        [FunctionName("update-favorites")]
        public static async Task<Entity> UpdateFavorites(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "update-favorites/{username}/{favoritesList}")] HttpRequest req,
            string username, string favoritesList,
            ILogger log)
        {
            // Get the user from the table
            BasicSamples basicSamples = new BasicSamples();
            Entity entity = await basicSamples.RunSamples(username);
            Entity newEntity=null;

            // Return a response to the user
            if (entity != null)
            {              
                newEntity = await basicSamples.RunSamplesForFavorites(username, favoritesList);
            }

            return newEntity;
        }


        private static async Task<T> ExtractContent<T>(HttpRequestMessage request)
        {
            string connectionRequestJson = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(connectionRequestJson);
        }


    }
}
