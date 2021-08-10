using System;

namespace CustomerLogIn
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using Microsoft.Azure.Documents;

    public class Common
    {
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }

            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }

            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }


        public static async Task<CloudTable> CreateTableAsync(string tableName)
        {
            // The connecting string of the storage account
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net";

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);

            return table;
        }


    }
}

