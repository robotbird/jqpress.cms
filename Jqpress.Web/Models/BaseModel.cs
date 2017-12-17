using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;
using Jqpress.Core.Template;


namespace Jqpress.Web.Models
{
    public class BaseModel
    {
        public string Css { get; set; }

        public class PostPageList : BasePageableModel { };

        /// <summary>
        /// 站点路径
        /// </summary>
        public string SiteUrl { get; set; }
        /// <summary>
        /// 站点路径
        /// </summary>
        public string SitePath { get; set; }
        /// <summary>
        /// 主题名称
        /// </summary>
        public string ThemeName { get; set; }
        /// <summary>
        /// 样式路径
        /// </summary>
        public string ThemeUrl { get; set; }
        /// <summary>
        /// 样式虚拟目录
        /// </summary>
        public string ThemePath { get; set; }
        /// <summary>
        /// 站点描述
        /// </summary>
        public string SiteDescription { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName {  get; set; }

        /// 是否首页
        /// </summary>
        public int IsDefault { get; set; }
        /// <summary>
        /// 是否post
        /// </summary>
        public int IsPost { get; set; }
        /// <summary>
        /// rss路径
        /// </summary>
        public string FeedUrl { get; set; }
        /// <summary>
        /// 评论rss路径
        /// </summary>
        public string FeedCommentUrl { get; set; }

        /// <summary>
        /// 当前url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 当前日期
        /// </summary>
        public string Date { get; set; }

        private string _searchkeyword;
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string SearchKeyword
        {
            get { return _searchkeyword ?? string.Empty; ; }
            set { _searchkeyword = value; }
        }

        #region 头部
        /// <summary>
        /// 标题
        /// </summary>
        public string PageTitle { get; set; }


        /// <summary>
        /// 关键词
        /// </summary>
        private string _metaKeywords;
        public string MetaKeywords
        {
            get { return _metaKeywords ?? (_metaKeywords =""); }
            set { _metaKeywords = value; }
        }


        /// <summary>
        /// 描述
        /// </summary>
        private string _metaDescription;
        public string MetaDescription
        {
            get { return _metaDescription ?? (_metaDescription =""); }
            set { _metaDescription = value; }
        }
        /// <summary>
        /// 头部wlwmanifest
        /// </summary>
        public string Head
        {
            get
            {
                string headhtml = string.Empty;

                headhtml += string.Format("<meta name=\"generator\" content=\"jqpress {0}\" />\n", "");
                headhtml += "<meta name=\"author\" content=\"jqpress Team\" />\n";
                headhtml += string.Format("<meta name=\"copyright\" content=\"2011-{0} jqpress Team.\" />\n", DateTime.Now.Year);
                headhtml += string.Format("<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{0}\"  href=\"{1}\"  />\n", "");
                headhtml += string.Format("<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"{0}xmlrpc/rsd.aspx\" />\n","");
                headhtml += string.Format("<link rel=\"wlwmanifest\" type=\"application/wlwmanifest+xml\" href=\"{0}xmlrpc/wlwmanifest.aspx\" />", "");

                return headhtml;
            }
        }
        #endregion

        #region 底部
        /// <summary>
        /// 底部版权信息
        /// </summary>
        public string FooterHtml { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        #endregion

        /// <summary>
        /// 导航
        /// </summary>
        public List<LinkInfo> NavLinks { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<TagInfo> RecentTags { get; set; }

        /// <summary>
        /// 普通链接
        /// </summary>
        public List<LinkInfo> GeneralLinks { get; set; }

    }
}