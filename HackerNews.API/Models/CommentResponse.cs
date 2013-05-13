using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class CommentResponse
    {
        public List<Comment> items { get; set; }
    }
}
