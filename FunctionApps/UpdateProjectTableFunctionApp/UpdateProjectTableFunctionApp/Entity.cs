using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateProjectTableFunctionApp
{
    using Microsoft.Azure.Cosmos.Table;
    public class Entity : TableEntity
    {
        public Entity()
        {
        }


        public Entity(string modelName, string id, string size, string location)
        {
            PartitionKey = modelName;
            RowKey = id;
            Size = size;
            Location = location;
        }


        public Entity(string modelName, string id)
        {
            PartitionKey = modelName;
            RowKey = id;
        }


        public string Size { get; set; }
        public string Location { get; set; }


    }
}
