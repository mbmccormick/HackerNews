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
        public string score { get; set; }
        public string user { get; set; }
        public string comments { get; set; }
        public string time { get; set; }
        public string item_id { get; set; }
        public string description { get; set; }

        public string body
        {
            get
            {
                return "Posted " + time + " by " + user;
            }
        }
    }
}
