using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateProjectTableFunctionApp
{
    using Microsoft.Azure.Cosmos.Table;
    public class ScanEntity : TableEntity
    {
        public ScanEntity(string partitionKeyParam, string itemNameParam, int scanCounterParam)
        {
            PartitionKey = partitionKeyParam;
            RowKey = itemNameParam;
            ScanCounter = scanCounterParam;
        }

        public int ScanCounter { get; set; }


    }
}
