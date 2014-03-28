using HackerNews.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Models
{
    public class CommentItem
    {
        public string id { get; set; }
        public int level { get; set; }
        public string user { get; set; }
        public string time_ago { get; set; }
        public string title { get; set; }
        public string content { get; set; }

        public CommentItem(Comment source)
        {
            id = source.id;
            level = source.level;
            user = source.user;
            time_ago = source.time_ago;
            title = source.title;
            content = source.content;
        }
    }
}
