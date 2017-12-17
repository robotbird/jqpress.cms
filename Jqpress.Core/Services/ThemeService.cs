using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using Jqpress.Core.Domain;
using Jqpress.Core.Configuration;
using Jqpress.Framework.Themes;

namespace Jqpress.Core.Services
{
 /// <summary>
    /// 主题管理
    /// </summary>
    public class ThemeService
    {
        public  ThemeInfo GetTheme(string xmlPath)
        {
            var localpath = HttpContext.Current.Server.MapPath(xmlPath);
            var theme = new ThemeInfo();
            theme.Name = "";
            theme.Author = "";
            theme.PubDate = "";
            theme.Version = "";
            theme.Email = "";
            theme.SiteUrl = "";
            theme.Thumbnail = xmlPath + @"/theme.jpg";

            if (!System.IO.File.Exists(localpath + @"\theme.xml"))
            {
                return theme;

            }
            try
            {
                XmlDocument xml = new XmlDocument();

                xml.Load(localpath + @"\theme.xml");

                theme.Name = xml.SelectSingleNode("theme/name").InnerText;
                theme.Author = xml.SelectSingleNode("theme/author").InnerText;
                theme.Email = xml.SelectSingleNode("theme/email").InnerText;
                theme.SiteUrl = xml.SelectSingleNode("theme/siteurl").InnerText;
                theme.PubDate = xml.SelectSingleNode("theme/pubdate").InnerText;
                theme.Version = xml.SelectSingleNode("theme/version").InnerText;
                return theme;
            }
            catch
            {
                return theme;
            }

        }
        /// <summary>
        /// 获取主题列表
        /// </summary>
        /// <returns></returns>
        public List<ThemeInfo> GetThemeList() 
        {
            var list = new List<ThemeInfo>();
            var folder = "/themes/";
            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(folder));
            foreach (DirectoryInfo d in dir.GetDirectories()) 
            {
                var theme = GetTheme(folder+d.Name);
                theme.Folder = d.Name;
                list.Add(theme);
            }
            return list;
        }

        //init the  theme
        public  void InitTheme(string theme)
        {
            //ThemeInfo themeInfo;
            IThemeContext themeContext = ThemeContext.GetInstance();
            var themeInfo = new ThemeInfo();
            themeInfo.Name = SiteConfig.GetSetting().Theme;

            if (!string.IsNullOrEmpty(theme))
            {
                themeInfo.Name = theme;
                themeContext.WorkingDesktopTheme = theme;
            }
            else if (!string.IsNullOrEmpty(themeInfo.Name))
            {
                themeContext.WorkingDesktopTheme = themeInfo.Name;
            }
            else
            {
                //默认样式
                themeContext.WorkingDesktopTheme = "Default";
            }
        }
    }
}
