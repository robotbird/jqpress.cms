using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;
using Jqpress.Web.Areas.Admin.Models;
using FileInfo =  Jqpress.Core.Domain.FileInfo;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class FilesController : BaseAdminController
    {
        public string urlpath = PressRequest.GetQueryString("path");
        public string rootpath = "/upfiles";

        private string FileName;

        /// <summary>
        /// 文件浏览
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            int pageIndex = PressRequest.GetQueryInt("page", 1);
            int pageSize = 50;
            if (string.IsNullOrEmpty(urlpath))
            {
                urlpath = rootpath;
            }
            var list = GetFilesList(urlpath);
            var model = new FilesModel();
            model.FoldList = list.FoldList.Select(fold => new FoldInfo
                                                              {
                                                                  FoldName = fold.FoldName,
                                                                  FoldPath = fold.FoldPath,
                                                                  FileSystemInfosLength = fold.FileSystemInfosLength
                                                              }).ToList();
            model.FileList = list.FileList.Select(f => new FileInfo
                                                           {
                                                               FileName = f.FileName,
                                                               Extension = f.Extension,
                                                               FileLength = f.FileLength,
                                                               FilePath = f.FilePath,
                                                               FileUrl = f.FileUrl
                                                           }).ToList();
            if (model.FileList.Count == 0)
            {
                IPagedList<FoldInfo> foldlist = new PagedList<FoldInfo>(model.FoldList, pageIndex - 1, pageSize);
                model.FoldPageList.LoadPagedList<FoldInfo>(foldlist);          
                model.FoldList = (List<FoldInfo>)foldlist;             
            }
            string controller = RouteData.Values["controller"].ToString();
            string aera = "Admin";
            model.CurrentAction = "/" + aera + "/" + controller + "/" + "list";
            model.UserDirectory = new DirectoryInfo(Server.MapPath(urlpath));
            ViewBag.path = urlpath;
            ViewBag.UrlPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            model.PathUrl = GetPathUrl();
            return View(model);
        }

        /// <summary>
        /// 文件浏览
        /// </summary>
        /// <returns></returns>
        public ActionResult FileList()
        {
            int pageIndex = PressRequest.GetQueryInt("page", 1);
            int pageSize = 50;
            if (string.IsNullOrEmpty(urlpath))
            {
                urlpath = rootpath;
            }
            var list = GetFilesList(urlpath);
            var model = new FilesModel();
            model.FoldList = list.FoldList.Select(fold => new FoldInfo
            {
                FoldName = fold.FoldName,
                FoldPath = fold.FoldPath,
                FileSystemInfosLength = fold.FileSystemInfosLength
            }).ToList();
            model.FileList = list.FileList.Select(f => new FileInfo
            {
                FileName = f.FileName,
                Extension = f.Extension,
                FileLength = f.FileLength,
                FilePath = f.FilePath,
                FileUrl = f.FileUrl
            }).ToList();
            if (model.FileList.Count == 0)
            {
                IPagedList<FoldInfo> foldlist = new PagedList<FoldInfo>(model.FoldList, pageIndex - 1, pageSize);
                model.FoldPageList.LoadPagedList<FoldInfo>(foldlist);
                model.FoldList = (List<FoldInfo>)foldlist;
            }
            string controller = RouteData.Values["controller"].ToString();
            string aera = "Admin";
            model.CurrentAction = "/" + aera + "/" + controller + "/" + "list";
            model.UserDirectory = new DirectoryInfo(Server.MapPath(urlpath));
            ViewBag.path = urlpath;
            ViewBag.UrlPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            model.PathUrl = GetPathUrl();
            return View(model);
        }

        /// <summary>
        /// 上传保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            var file = Request.Files["FileUpload"];
            string basepath = PressRequest.GetQueryString("path");
            if (file != null)
            {
                var ext = FileHelper.GetFileExtName(file.FileName);
                var filename = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + ext;  
                if (string.IsNullOrEmpty(urlpath))
                {
                    urlpath = rootpath;
                }
                var savepath = Server.MapPath(urlpath + "/" + filename);

                if (FileHelper.FileExists(savepath))
                {
                    return Content("<script>alert('此文件名已存在');location.href=\"/admin/files/list?path=" + urlpath + "\";</script>");
                }
                else 
                {
                    try 
                    {
                        file.SaveAs(savepath);                                    
                    } catch(Exception e){
                        return Content("<script>alert('文件格式不正确，请确认图片是否能打开');location.href=\"/admin/files/list?path=" + urlpath + "\";</script>");                      
                    }
                }

                return Content("<script>location.href=\"/admin/files/list?path="+urlpath+"\"; </script>");
            }
            return JavaScript("<script>alert('上传失败');</script>");
        }

        public ContentResult CreateFolder() 
        {
            var root = "/upfiles/";
            var foldername = PressRequest.GetQueryString("folder").Trim();
            if (!string.IsNullOrEmpty(foldername)) 
            {
                FileHelper.CreateDir(Server.MapPath(root+foldername));
                return Content("创建成功");
            }
            return Content("创建失败");

        }

        /// <summary>
        /// 删除路径下相应文件
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteFile()
        {
            string path = PressRequest.GetQueryString("path").Trim();
            string url = PressRequest.GetQueryString("url").Trim();
            if (System.IO.File.Exists(path) || Directory.Exists(path))
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(path);
                    file.Delete();
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        di.Delete(true);
                    }
                }
            }
            var fromurl = Request.UrlReferrer.ToString();
            return Redirect(fromurl);
        }

        public FilesModel GetFilesList(string path)
        {
            path = path.Replace("//", "/");
            var model = new FilesModel();
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
                        FileSystemInfosLength = ((DirectoryInfo)d).GetFileSystemInfos().Length
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
                        FilePath = d.FullName
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
                pathLink += string.Format("/<a target='ifrmfile' href='/admin/files/{2}?path={0}'>{1}</a>", temp, tempPath[i], FileName);
            }
            return pathLink.Replace("//", "");
        }
    }
}
