using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Core.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Services;
using Jqpress.Framework.Utils;
using Jqpress.Web.Models;

namespace Jqpress.Web.Controllers
{
    public class InstallController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Setup()
        {
            return View();
        }

        [HttpPost,ActionName("Setup")]
        public ActionResult Setup(InstallModel model)
        {
                if (string.IsNullOrEmpty(model.SiteName))
                {
                    ViewData["Err_SiteName"] = "请输入站点名称";
                }
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ViewData["Err_UserName"] = "请输入用户名";
                }
                if (string.IsNullOrEmpty(model.PassWord))
                {
                    ViewData["Err_PassWord"] = "请输入密码";
                }
                if (model.PassWord!=model.ConfirmPassword)
                {
                    ViewData["Err_ConfirmPassword"] = "两次密码不一致";
                }
                if (ViewData.Count==0)
                {
                    var config = SiteConfig.GetSetting();
                    config.SiteName = model.SiteName;
                    config.MetaKeywords = model.SiteName;
                    config.MetaDescription = model.SiteName;
                    config.IsInstalled = true;
                    SiteConfig.UpdateSetting();
                    var userService = new UserService();
                    if (userService.GetUser(model.UserName)==null)
                    {
                        var user = new UserInfo
                            {
                                UserName = model.UserName,
                                Password = EncryptHelper.MD5(model.PassWord), 
                                Role = (int)UserRole.Administrator
                            };
                        userService.InsertUser(user);
                    }
                    return View("Success");
                }
                else
                {
                    return View();
                }
        }
    }
}
