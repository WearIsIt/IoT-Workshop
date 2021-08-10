using System;
using System.Collections.Generic;
using System.Text;

namespace UploadTagsToStroage
{
    using Microsoft.Azure.Cosmos.Table;
    public class Entity : TableEntity
    {
        public Entity()
        {
        }

        public Entity(string tagName, string itemNames)
        {
            PartitionKey = tagName;
            RowKey = "1";
            Items = itemNames;

        }

        public string Items { get; set; }


    }
}