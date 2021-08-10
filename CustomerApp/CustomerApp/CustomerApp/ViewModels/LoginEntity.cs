using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.ViewModels
{
    using Microsoft.Azure.Cosmos.Table;
    public class LoginEntity : TableEntity
    {
        public LoginEntity()
        {
        }

        public LoginEntity(string const1, string username)
        {
            PartitionKey = const1;
            RowKey = username;

        }
        public string Password { get; set; }
        public string Favorites { get; set; }


    }
}
