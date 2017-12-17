using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileLength { get; set; }
        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件url地址
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        }
}
