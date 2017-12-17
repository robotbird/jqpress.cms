using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jqpress.Web.Models
{
    public class InstallModel
    {
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        public string ConfirmPassword { get; set; }
    }
}