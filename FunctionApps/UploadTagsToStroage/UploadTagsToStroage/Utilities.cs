using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UploadTagsToStroage
{
    class Utilities
    {
        public static CloudTable initTable()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference("TagsTable");
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


        public static async Task<HashSet<string>> getItemsAsync(string[] tags, int numOfTags, CloudTable table)
        {
            HashSet<string> hashItems = new HashSet<string>();
            Entity entity;
            foreach (string tag in tags)
            {
                entity = await RetrieveEntityUsingPointQueryAsync(table, tag, "1");
                if (entity != null)
                {              
                    insertItemsToHashSet(entity.Items, hashItems);                   
                }
            }
            return hashItems;
        }


        public static void insertItemsToHashSet(string items, HashSet<string> hashItems)
        {
            string[] itemsPerTag = items.Split('_');
            foreach (string item in itemsPerTag)
            {
                hashItems.Add(item);
            }
        }


    }
}
