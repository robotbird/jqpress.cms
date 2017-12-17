using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class ThemeListModel
    {
        public ThemeListModel() 
        {
            ThemeList = new List<ThemeInfo>();
        }

        public List<ThemeInfo> ThemeList { get; set; }
        /// <summary>
        /// 当前使用的样式
        /// </summary>
        public string CurrentTheme { get; set; }
    }
}