using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Configuration;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 标签实体
    /// </summary>
    public class TagInfo : IComparable<TagInfo>
    {

        public int CompareTo(TagInfo other)
        {
            if (this.SortNum != other.SortNum)
            {
                return this.SortNum.CompareTo(other.SortNum);
            }
            return this.TagId.CompareTo(other.TagId);
        }

        #region 非字段

        private string _url;
        private string _link;

        /// <summary>
        /// 地址
        /// </summary>
        public string Url
        {
            //set { _url = value; }
            //get { return _url; }
            get
            {
                return string.Format("{0}tag/{1}", ConfigHelper.SiteUrl, Jqpress.Framework.Web.HttpHelper.UrlEncode(this.PageName));
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        public string Link
        {
            //set { _link = value; }
            //get { return _link; }
            get
            {
                return string.Format("<a href=\"{0}\" title=\"标签:{1}\">{2}</a>", this.Url, this.CateName, this.CateName);
            }
        }

        #endregion


        /// <summary>
        /// ID
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string CateName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
