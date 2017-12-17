using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// rss新闻
    /// </summary>
   public class FeedInfo
    {
        public string title { get; set; }
        public string link { get; set; }
        public string guid { get; set; }
        public string author { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public string pubDate { get; set; }
    }
}
