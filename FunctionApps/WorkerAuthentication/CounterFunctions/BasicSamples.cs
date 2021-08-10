using System;

namespace ProjectFunctions
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;

    public class BasicSamples
    {
        public async Task<Entity> RunSamples(string username)
        {
            string tableName = "LoginTable";

            // Create or reference an existing table
            CloudTable table = await Common.CreateTableAsync(tableName);

            try
            {
            //  Get the existing user from the table
                Entity entity = await getFromTable(table, username);
                return entity;
            }

            finally
            {    
            }
        }


        private static async Task<Entity> getFromTable(CloudTable table, string username)
        {
            // read the first entity using a point query 
            Entity entity = await SamplesUtils.RetrieveEntityUsingPointQueryAsync(table, "1", username);
            return entity;
        }


    }
}