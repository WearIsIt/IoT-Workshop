using System;

namespace CustomerLogIn
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;

    public class BasicSamples
    {
        public async Task<Entity> RunSamples(string username)
        {
            string tableName = "CustomerLoginTable";

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


        public async Task<Entity> RunSamplesForFavorites(string username, string favorites)
        {
            string tableName = "CustomerLoginTable";

            // Create or reference an existing table
            CloudTable table = await Common.CreateTableAsync(tableName);

            try
            {
                //  get the existing user from the table
                Entity entity = await getFromTable(table, username);
                Entity newEntity = await updateInTable(table, entity, favorites);
                return newEntity;
            }

            finally
            {
            }
        }


        public async Task<Entity> RunSamplesForSignUp(Entity entity)
        {
            // Create or reference an existing table
            CloudTable table = await Common.CreateTableAsync("CustomerLoginTable");
            Entity entityNew;
            try
            {
                // Check if email is already in storage
                entityNew = await getFromTable(table, entity.RowKey); 
                if (entityNew!=null)
                {
                    return null;
                }

                //  Get the existing user from the table
                entityNew = await updateInTable(table, entity, "NoItems");
                return entityNew;
            }

            finally
            {
            }
        }


        private static async Task<Entity> updateInTable(CloudTable table, Entity entity, string favorites)
        {
            // Create an instance of a customer entity.
            Entity newEntity = new Entity("2", entity.RowKey)
            {
                Password = entity.Password,
                Favorites = favorites,
            };
            
            // Insert the entity to the table (override the existing entity)
            newEntity = await SamplesUtils.InsertOrMergeEntityAsync(table, newEntity);
            return newEntity;
        }


        private static async Task<Entity> getFromTable(CloudTable table, string username)
        {
            // Read the first entity using a point query 
            Entity entity = await SamplesUtils.RetrieveEntityUsingPointQueryAsync(table, "2", username);
            return entity;
        }


    }
}