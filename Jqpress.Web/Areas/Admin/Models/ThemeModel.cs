using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;
using FileInfo = Jqpress.Core.Domain.FileInfo;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class ThemeModel
    {
        public ThemeModel() 
        {
            FoldList = new List<FoldInfo>();
            FileList = new List<FileInfo>();
        }

        /// <summary>
        /// 文件夹列表
        /// </summary>
        public List<FoldInfo> FoldList { get; set; }
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileInfo> FileList { get; set; }
        /// <summary>
        /// 当前路径
        /// </summary>
        public string PathUrl { get; set; }
    }
}