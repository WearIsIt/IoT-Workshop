using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomerLogIn
{
    public class SamplesUtils
    {
        public static async Task<Entity> InsertOrMergeEntityAsync(CloudTable table, Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                Entity insertedCustomer = result.Result as Entity;

                return insertedCustomer;
            }

            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }


        public static async Task<Entity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Entity>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                if (result.Result == null)
                {
                    return null;
                }
                Entity entity = result.Result as Entity;

                return entity;
            }

            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }


    }
}
