using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScanCountFunctionApp
{
    class Utilities
    {
        public static CloudTable initTable(string tableName)
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);
            return table;           
        }


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


        public static void DeleteEntity(CloudTable table, string partitionKey, string rowKey)
        {
            var entity = new DynamicTableEntity(partitionKey, rowKey) { ETag = "*" };
            table.Execute(TableOperation.Delete(entity));
        }


        public static async Task SwapEntitiesAsync(CloudTable table, Entity entityToRemove, Entity entityToAdd)
        {
            // Delete entityToRemove
            DeleteEntity(table, entityToRemove.PartitionKey, entityToRemove.RowKey);

            // Add entityToAdd to Top Five (with partitionKey "2")
            await InsertOrMergeEntityAsync(table, entityToAdd);
        }


    }
}
