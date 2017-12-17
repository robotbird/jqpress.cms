using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 统计实体
    /// </summary>
    public class StatisticsInfo
    {
        /// <summary>
        /// 文章数
        /// </summary>
        public int PostCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 访问数
        /// </summary>
        public int VisitCount { get; set; }
        /// <summary>
        /// 标签数
        /// </summary>
        public int TagCount { get; set; }
    }
}
