using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Models
{
    public class Response
    {
        public object nextId { get; set; }
        public List<Item> items { get; set; }
        public string version { get; set; }
        public DateTime cachedOnUTC { get; set; }
    }
}
