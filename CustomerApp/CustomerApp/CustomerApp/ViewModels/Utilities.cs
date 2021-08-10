using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace CustomerApp.ViewModels
{
    class Utilities
    {
        public static string getPasswordAsync(string username)
        {
            CloudTable table = initTable();

            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<LoginEntity>("1", username);
                TableResult result = table.Execute(retrieveOperation);
                LoginEntity loginEntity = result.Result as LoginEntity;
                return loginEntity.Password;
            }

            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }


        public static CloudTable initTable()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference("LoginTable");
            return table;
        }


    }
}

