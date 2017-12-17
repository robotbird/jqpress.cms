using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.Configuration;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 连接实体(包括导航和友情连接)
    /// </summary>
    public class LinkInfo : IComparable<LinkInfo>
    {
        public int CompareTo(LinkInfo other)
        {
            if (this.SortNum != other.SortNum)
            {
                return this.SortNum.CompareTo(other.SortNum);
            }
            return this.LinkId.CompareTo(other.LinkId);
        }
        
        #region 非字段
        private string _linkurl;
        /// <summary>
        /// 连接Url
        /// </summary>
        public string LinkUrl
        {
            set { _linkurl = value; }
            get
            {
                return _linkurl.Replace("${siteurl}", ConfigHelper.SiteUrl);
            }
        }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string Link
        {
            get
            {
                return string.Format("<a href=\"{0}\" title=\"{1}\" target=\"{2}\">{3}</a>", this.LinkUrl, this.Description, this.Target, this.LinkName);
            }
        }
        #endregion

        /// <summary>
        /// ID
        /// </summary>
        public int LinkId { get; set; }

        /// <summary>
        /// 类型(待用,现默认为0)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// 打开方式
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 状态(1:显示,0:隐藏)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
