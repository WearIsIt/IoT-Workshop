using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;


namespace LoginViewSample.Core.Settings
{
    public class PostRequest
    {
        public PostRequest(string itemNameParam, string tagsParam)
        {
            itemName = itemNameParam;
            tags = tagsParam;
        }

        public string itemName { get; set; }
        public string tags { get; set; }
    }
}



