using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class IndexModel
    {
        /// <summary>
        /// 文章数量
        /// </summary>
        public int PostCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 标签数量
        /// </summary>
        public int TagCount { get; set; }
        /// <summary>
        /// 访问量
        /// </summary>
        public int VisitCount { get; set; }
        /// <summary>
        /// 数据库路径
        /// </summary>
        public string DbPath { get; set; }
        /// <summary>
        /// 数据库大小
        /// </summary>
        public string DbSize { get; set; }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string UpfilePath { get; set; }
        /// <summary>
        /// 上传文件个数
        /// </summary>
        public int UpfileCount { get; set; }
        /// <summary>
        /// 上传文件大小
        /// </summary>
        public string UpfileSize { get; set; }
        /// <summary>
        /// 缓存数量
        /// </summary>
        public int CacheCount { get; set; }
        /// <summary>
        /// 程序版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 评论列表
        /// </summary>
        public List<CommentInfo> Commentlist { get; set; }
    }
}