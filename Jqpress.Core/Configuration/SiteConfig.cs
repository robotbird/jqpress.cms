using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;
using System.Web;

namespace Jqpress.Core.Configuration
{
    /// <summary>
    /// 配置管理
    /// </summary>
    public class SiteConfig
    {
        static string SiteConfigPath = HttpContext.Current.Server.MapPath("~/site.config");
        /// <summary>
        /// 静态变量
        /// </summary>
        private static SiteConfigInfo _setting;

        /// <summary>
        /// lock
        /// </summary>
        private static object lockHelper = new object();

        static SiteConfig()
        {
            LoadSetting();
        }

        /// <summary>
        /// 单例初始化
        /// </summary>
        public static void LoadSetting()
        {
            if (_setting == null)
            {
                lock (lockHelper)
                {
                    if (_setting == null)
                    {
                        object obj = Jqpress.Framework.Xml.SerializationHelper.Load(typeof(SiteConfigInfo), SiteConfigPath);
                        if (obj == null)
                        {
                            _setting= new SiteConfigInfo();
                        }

                        _setting= (SiteConfigInfo)obj;
                    }
                }
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        public static SiteConfigInfo GetSetting()
        {
            return _setting;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public static bool UpdateSetting()
        {
           bool result= Jqpress.Framework.Xml.SerializationHelper.Save(_setting, SiteConfigPath);
           return result;
        }

    }
}
