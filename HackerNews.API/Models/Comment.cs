using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class Comment
    {
        public string username { get; set; }
        public string comment { get; set; }
        public string id { get; set; }
        public int grayedOutPercent { get; set; }
        public string reply_id { get; set; }
        public string time { get; set; }
        public List<Comment> children { get; set; }

        public string title
        {
            get
            {
                return username + " " + time;
            }
        }
    }
}
