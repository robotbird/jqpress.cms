using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;
using FileInfo = Jqpress.Core.Domain.FileInfo;


namespace Jqpress.Web.Areas.Admin.Models
{
    public class FilesModel : BaseModel
    {
        public FilesModel()
        {
            filePageList = new FilePageList();
            FoldPageList = new PageList();
            FoldList = new List<FoldInfo>();
            FileList = new List<FileInfo>();
        }
        /// <summary>
        /// 父页面的action
        /// </summary>
        public string CurrentAction { get; set; }
        /// <summary>
        /// 允许上传格式
        /// </summary>
        public string AllowFileExtension { get; set; }
        /// <summary>
        /// 当前路径
        /// </summary>
        public string PathUrl { get; set; }

        /// <summary>
        /// 分页
        /// </summary>
        public FilePageList filePageList { get; set; }
        /// <summary>
        /// 分页列表
        /// </summary>
        public class FilePageList : BasePageableModel { }
        /// <summary>
        /// 用户文件夹
        /// </summary>
        public DirectoryInfo UserDirectory { get; set; }

        /// <summary>
        /// 获取文件对应的图标
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public string GetFileImage(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".gif":
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".tif":
                    return "jpg.gif";

                case ".doc":
                case ".docx":
                case ".rtf":
                    return "doc.gif";
                case ".ppt":
                case ".pptx":
                    return "ppt.gif";
                case ".xls":
                case ".xlsx":
                case ".csv":
                    return "xls.gif";
                case ".pdf":
                    return "pdf.gif";

                case ".rar":
                case ".zip":
                case ".cab":
                case ".7z":
                    return "rar.gif";

                case ".wav":
                case ".wmv":
                case ".wma":
                case ".mpeg":
                case ".avi":
                case ".mp3":
                    return "wma.gif";

                case ".ini":
                case ".txt":
                case ".css":
                case ".js":
                case ".htm":
                case ".html":
                case ".xml":
                case ".h":
                case ".c":
                case ".php":
                case ".vb":
                case ".cpp":
                case ".cs":
                case ".aspx":
                case ".asm":
                case ".sln":
                case ".vs":
                    return "txt.gif";

                case ".fla":
                case ".flv":
                case ".swf":
                    return "swf.gif";

                case ".psd":
                    return "psd.gif";

                case ".chm":
                    return "chm.gif";

                case ".dll":
                case ".exe":
                case ".msi":
                case ".db":
                    return "exe.gif";

                default: return "default.gif";
            }
        }

        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public bool IsImage(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.ToLower();
            }
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp" || ext == ".png")
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 分页列表
        /// </summary>
        public PageList FoldPageList { get; set; }
        /// <summary>
        /// 分页类
        /// </summary>
        public class PageList : BasePageableModel { }
        /// <summary>
        /// 文件夹列表
        /// </summary>
        public List<FoldInfo> FoldList { get; set; }
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileInfo> FileList { get; set; }
    }
}