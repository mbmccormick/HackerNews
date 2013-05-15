using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class Comment
    {
        public string id { get; set; }
        public int level { get; set; }
        public string user { get; set; }
        public string time_ago { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public List<Comment> comments { get; set; }
    }
}
