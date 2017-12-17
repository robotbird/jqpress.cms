using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class PostListModel:BaseModel
    {
        public PostListModel()
        {
            PostList = new List<PostInfo>();
            PageList = new PostPageList();
            CateSelectItem = new List<SelectListItem>();
        }
        /// <summary>
        /// post list
        /// </summary>
        public List<PostInfo> PostList { get;set;}

        public PostPageList PageList { get; set; }

        /// <summary>
        /// post's category selectitem
        /// </summary>
        public List<SelectListItem> CateSelectItem { get; set; }

        /// <summary>
        /// 文章列表信息(作者,分类等)
        /// </summary>
        public string PostMessage { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
    }
}