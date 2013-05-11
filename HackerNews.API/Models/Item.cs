using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class Item
    {
        public string title { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public int commentCount { get; set; }
        public int points { get; set; }
        public string postedAgo { get; set; }
        public string postedBy { get; set; }

        public string description
        {
            get
            {
                return "Posted " + postedAgo + " by " + postedBy;
            }
        }
    }
}
