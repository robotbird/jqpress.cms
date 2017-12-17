using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Schema;
using System.IO;
using Jqpress.Core.Common;
using Jqpress.Core.Domain;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Services;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Configuration;
using Jqpress.Web.Areas.Admin.Models;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public  class HomeController : Controller
    {
        #region private items
        private StatisticsService _statisticsService = new StatisticsService();
        private CommentService _commentService = new CommentService();
        private UserService _userService = new UserService();
        #endregion;
        #region 首页
        private int UpfileCount = 0;

        /// <summary>
        /// 控制台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new IndexModel();

            model.PostCount = _statisticsService.GetStatistics().PostCount;
            model.CommentCount = _statisticsService.GetStatistics().CommentCount;
            model.TagCount = _statisticsService.GetStatistics().TagCount;
            model.VisitCount = _statisticsService.GetStatistics().VisitCount;
            model.Commentlist = _commentService.GetCommentList(15, 1, -1, -1, -1, (int)ApprovedStatus.Wait, -1, string.Empty);
            model.DbPath = ConfigHelper.SitePath + ConfigHelper.DbConnection;

            System.IO.FileInfo file = new System.IO.FileInfo(Server.MapPath(ConfigHelper.SitePath + ConfigHelper.DbConnection));
            model.DbSize = GetFileSize(file.Length);

            model.UpfilePath = ConfigHelper.SitePath + "upfiles";

           // GetDirectorySize(Server.MapPath(model.UpfilePath));

            //model.UpfileSize = GetFileSize(dirSize);// mono

            //GetDirectoryCount(Server.MapPath(model.UpfilePath));//
            model.UpfileCount = UpfileCount;

            return View(model);
        }

        /// <summary>
        /// 获取rss结果
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRss()
        {
            string tmpl = string.Empty;
            List<FeedInfo> listrss = new List<FeedInfo>();
            try
            {
                string rssurl = "http://www.jqpress.com/feed/post.aspx";
                XmlTextReader reader = new XmlTextReader(rssurl);
                DataSet ds = new DataSet();
                ds.ReadXml(reader);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[2];

                    foreach (DataRow dr in dt.Rows)
                    {
                        FeedInfo post = new FeedInfo();
                        post.title = dr["title"].ToString();
                        post.description = dr["description"].ToString();
                        post.link = dr["link"].ToString();
                        post.pubDate = dr["pubDate"].ToString();
                        listrss.Add(post);
                    }
                }

            }
            catch (Exception e) { }

            for (int i = 0; i < (listrss.Count > 4 ? 4 : listrss.Count); i++)
            {
                tmpl += "<a title=" + listrss[i].description + " href=" + listrss[i].link + " class=rsswidget>" + listrss[i].title + "</a>";
                tmpl += "<span class=rss-date>" + Jqpress.Framework.Utils.DateTimeHelper.DateToChineseString(Convert.ToDateTime(listrss[i].pubDate)) + "</span>";
                tmpl += "<div class=rssSummary>" + Jqpress.Framework.Utils.StringHelper.CutString(listrss[i].description, 0, 80) + " […]</div>";
            }
            return Content(tmpl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">byte</param>
        /// <returns></returns>
        protected string GetFileSize(long size)
        {
            string FileSize = string.Empty;
            if (size > (1024 * 1024 * 1024))
                FileSize = ((double)size / (1024 * 1024 * 1024)).ToString(".##") + " GB";
            else if (size > (1024 * 1024))
                FileSize = ((double)size / (1024 * 1024)).ToString(".##") + " MB";
            else if (size > 1024)
                FileSize = ((double)size / 1024).ToString(".##") + " KB";
            else if (size == 0)
                FileSize = "0 Byte";
            else
                FileSize = ((double)size / 1).ToString(".##") + " Byte";

            return FileSize;
        }

        /// <summary>
        /// 文件夹大小
        /// </summary>
        public long dirSize = 0;

        /// <summary>
        /// 递归文件夹大小
        /// </summary>
        /// <param name="dirp"></param>
        /// <returns></returns>
        private long GetDirectorySize(string dirp)
        {
            DirectoryInfo mydir = new DirectoryInfo(dirp);
            foreach (FileSystemInfo fsi in mydir.GetFileSystemInfos())
            {
                if (fsi is System.IO.FileInfo)
                {
                    System.IO.FileInfo fi = (System.IO.FileInfo)fsi;
                    dirSize += fi.Length;
                }
                else
                {
                    DirectoryInfo di = (DirectoryInfo)fsi;
                    string new_dir = di.FullName;
                    GetDirectorySize(new_dir);
                }
            }
            return dirSize;
        }

        /// <summary>
        /// 递归文件数量
        /// </summary>
        /// <param name="dirp"></param>
        /// <returns></returns>
        private int GetDirectoryCount(string dirp)
        {
            DirectoryInfo mydir = new DirectoryInfo(dirp);
            foreach (FileSystemInfo fsi in mydir.GetFileSystemInfos())
            {
                if (fsi is System.IO.FileInfo)
                {
                    //   FileInfo fi = (FileInfo)fsi;
                    UpfileCount += 1;
                }
                else
                {
                    DirectoryInfo di = (DirectoryInfo)fsi;
                    string new_dir = di.FullName;
                    GetDirectoryCount(new_dir);
                }
            }
            return UpfileCount;
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginOn()
        {
            if (VerifyLogin())
            {
                return RedirectToAction("index", "home");
            }
            return View("login");
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        public bool VerifyLogin()
        {
            UserInfo user = null;
            string userName = PressRequest.GetFormString("username");
            string password = Jqpress.Framework.Utils.EncryptHelper.MD5(PressRequest.GetFormString("password"));
            int expires = PressRequest.GetFormString("rememberme") == "forever" ? 43200 : 0;

            user = _userService.GetUser(userName, password);

            if (user != null)
            {
                if (user.Status == 0)
                {
                    ModelState.AddModelError("", "此用户已停用");
                }
                _userService.WriteUserCookie(user.UserId, user.UserName, user.Password, expires);
                return true;
            }
            else
            {
                ModelState.AddModelError("", "用户名或密码错误!");
            }
            return false;
        }
        #endregion


        #region 用户
        #endregion

        #region 链接
        #endregion

        #region 设置
        #endregion

        #region 工具
        #endregion
    }
}
