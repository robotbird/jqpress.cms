using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Models
{
    public class PostListModel : BaseModel
    {
        public PostListModel()
        {
            PostList = new List<PostInfo>();
            PageList = new PostPageList();
            Category = new CategoryInfo();
        }
        public PostPageList PageList { get; set; }

        /// <summary>
        /// 文章列表信息(作者,分类等)
        /// </summary>
        public string PostMessage { get; set; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<PostInfo> PostList { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 当前分类
        /// </summary>
        public CategoryInfo Category { get; set; }
    }
}