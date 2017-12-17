using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Core.Domain;


namespace Jqpress.Web.Areas.Admin.Models
{
    public class PostModel
    {
        public PostModel() 
        {
            Post = new PostInfo();
            CateSelectItem = new List<SelectListItem>();
            TagList = new List<TagInfo>();
        }
        /// <summary>
        /// post
        /// </summary>
        public PostInfo Post { get; set; }
        /// <summary>
        /// post's category selectitem
        /// </summary>
        public List<SelectListItem> CateSelectItem { get; set; }
        /// <summary>
        /// tag list
        /// </summary>
        public List<TagInfo> TagList { get; set; }
    }
}