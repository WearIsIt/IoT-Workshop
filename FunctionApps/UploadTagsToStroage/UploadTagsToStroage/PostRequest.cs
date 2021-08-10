using System;
using System.Collections.Generic;
using System.Text;

namespace UploadTagsToStroage
{
    public class PostRequest
    {
        public PostRequest(string itemNameParam, string tagsListParam)
        {
            itemName = itemNameParam;
            tagsList = tagsListParam;
        }

        public string itemName { get; set; }
        public string tagsList { get; set; }


    }
}
