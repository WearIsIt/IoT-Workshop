using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.ViewModels
{
    using Microsoft.Azure.Cosmos.Table;
    public class ItemEntity : TableEntity
    {
        public ItemEntity()
        {
        }


        public ItemEntity(string modelName, string id, string size, string location)
        {
            PartitionKey = modelName;
            RowKey = id;
            Size = size;
            Location = location;
        }


        public string Size { get; set; }
        public string Location { get; set; }


    }
}