using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Models
{
    public class PostModel:BaseModel
    {
        /// <summary>
        /// 是否开启验证码
        /// </summary>
        public int EnableVerifyCode { get; set; }
        /// <summary>
        /// 文章
        /// </summary>
        public PostInfo Post { get; set; }
        /// <summary>
        /// 评论列表
        /// </summary>
        public List<CommentInfo> Comments { get; set; }
        /// <summary>
        /// 分页字符串
        /// </summary>
        public string Pager { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
    }
}