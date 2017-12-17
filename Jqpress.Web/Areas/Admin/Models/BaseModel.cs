using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;


namespace Jqpress.Web.Areas.Admin.Models
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
    }
}