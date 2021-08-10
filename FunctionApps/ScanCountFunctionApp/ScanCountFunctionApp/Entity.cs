using System;
using System.Collections.Generic;
using System.Text;

namespace ScanCountFunctionApp
{
    using Microsoft.Azure.Cosmos.Table;
    public class Entity : TableEntity
    {
        public Entity()
        {
        }

        public Entity(string partitionKeyParam, string itemNameParam)
        {
            PartitionKey = partitionKeyParam;
            RowKey = itemNameParam;
        }

        public Entity(string partitionKeyParam, string itemNameParam, int scanCounterParam)
        {
            PartitionKey = partitionKeyParam;
            RowKey = itemNameParam;
            ScanCounter = scanCounterParam;
        }


        public int ScanCounter { get; set; }


    }
}