using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;
using Jqpress.Core.Services;
using Jqpress.Core.Configuration;
using Jqpress.Web.Areas.Admin.Models;
using FileInfo = Jqpress.Core.Domain.FileInfo;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class OptionController : BaseAdminController
    {
        private string FileName;
        public string urlpath = PressRequest.GetQueryString("path");
        public string rootpath = "/upfiles";

        public ActionResult Site() 
        {
            var config = SiteConfig.GetSetting();
            return View(config);
        }

        /// <summary>
        /// 样式
        /// </summary>
        /// <returns></returns>
        public ActionResult Themes() 
        {
            ThemeService themeService = new ThemeService();
            var config = SiteConfig.GetSetting();

            var model = new ThemeListModel();
            model.ThemeList = themeService.GetThemeList();
            model.CurrentTheme = config.Theme;

            return View(model);
        }
        /// <summary>
        /// 主题编辑
        /// </summary>
        /// <param name="themename"></param>
        /// <returns></returns>
        public ActionResult ThemeEdit(string themename) 
        {


            var model = new ThemeModel();
            if (!string.IsNullOrEmpty(themename)) 
            {
                var path = "/themes/"+themename;
                if (!string.IsNullOrEmpty(urlpath)) 
                {
                    path = urlpath;
                }
                model = GetFilesList(path);
                if (!string.IsNullOrEmpty(urlpath))
                {
                    model.PathUrl = GetPathUrl();
                }

            }
            ViewBag.UrlPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            return View(model);
        }
        public ThemeModel GetFilesList(string path)
        {
            path = path.Replace("//", "/");
            var model = new ThemeModel();
            if (!Directory.Exists(Server.MapPath(path)))
                return model;

            var dic = new DirectoryInfo(Server.MapPath(path));
            foreach (var d in dic.GetFileSystemInfos())
            {
                if (d is DirectoryInfo)
                {
                    var fold = new FoldInfo
                    {
                        FoldName = d.Name,
                        FoldPath = path,
                        FileSystemInfosLength = ((DirectoryInfo)d).GetFileSystemInfos().Length,
                        UpdateTime = d.LastWriteTime
                    };
                    model.FoldList.Add(fold);
                }
                else
                {
                    var file = new FileInfo
                    {
                        FileName = d.Name,
                        FileLength = FileHelper.ConvertUnit(((System.IO.FileInfo)d).Length),
                        Extension = d.Extension,
                        FileUrl = path + "/" + d.Name,
                        FilePath = d.FullName,
                        UpdateTime = d.LastWriteTime
                    };
                    model.FileList.Add(file);
                }
            }
            return model;
        }

        /// <summary>
        /// 获取当前路径所有连接
        /// </summary>
        protected string GetPathUrl()
        {
            string pathLink = string.Empty;
            FileName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            urlpath = urlpath.Replace("//", "");
            string path2 = urlpath.Substring(1, urlpath.Length - 1);

            string[] tempPath = path2.Split('/');

            string temp = "/";

            pathLink = ConfigHelper.SitePath.TrimEnd('/');

            for (int i = 0; i < tempPath.Length; i++)
            {

                temp += (i != 0 ? "/" : "") + tempPath[i];
                if (i == 0 && ConfigHelper.SitePath.Length > 1) //有虚拟目录
                {
                    continue;
                }
                pathLink += string.Format("/<a  href='/admin/option/{2}?themename={2}&path={0}'>{1}</a>", temp, tempPath[i], FileName,Request["themename"]);
            }
            return pathLink.Replace("//", "");
        }


        /// <summary>
        /// 更新配置
        /// </summary>
        /// <returns></returns>
        [HttpPost, ActionName("SaveConfig"), ValidateInput(false)]
        public ActionResult SaveConfig(SiteConfigInfo configInfo) 
        {
            var  config= SiteConfig.GetSetting();
            config.SiteName = configInfo.SiteName;
            config.MetaKeywords = configInfo.MetaKeywords;
            config.MetaDescription = configInfo.MetaDescription;
            config.FooterHtml = configInfo.FooterHtml;
            SiteConfig.UpdateSetting();
            SuccessNotification("保存成功");
            return RedirectToAction("site");
        }

        /// <summary>
        /// 保存主题
        /// </summary>
        /// <returns></returns>
        public ActionResult UseTheme()
        {
            var themename = PressRequest.GetQueryString("themename");
            var config = SiteConfig.GetSetting();
            config.Theme = themename;
            SiteConfig.UpdateSetting();
            SuccessNotification("设置成功");
            return RedirectToAction("themes");
        }

    }
}
