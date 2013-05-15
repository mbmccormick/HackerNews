using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class CommentResponse
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string domain { get; set; }
        public string description { get; set; }
        public int points { get; set; }
        public string user { get; set; }
        public string time_ago { get; set; }
        public int comments_count { get; set; }
        public object content { get; set; }
        public object poll { get; set; }
        public string type { get; set; }
        public List<Comment> comments { get; set; }
        public object more_comments_id { get; set; }
    }
}
