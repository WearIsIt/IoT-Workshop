using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApp.ViewModels
{
    public class TopEntity
    {
        public TopEntity(string itemNameParam, int rankParam, int scoreParam)
        {
            itemName = itemNameParam;
            rank = rankParam;
            score = scoreParam;
        }

        public string itemName { get; set; }
        public int rank { get; set; }
        public int score { get; set; }
        public string image { get; set; }

    }
}
