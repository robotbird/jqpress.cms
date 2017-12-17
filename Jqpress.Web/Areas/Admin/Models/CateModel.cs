using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class CateModel
    {
        public CateModel() 
        {
            Category = new CategoryInfo();
            CateSelectItem = new List<SelectListItem>();
            CateType = new List<SelectListItem>();
            CateList = new List<CategoryInfo>();
        }
        /// <summary>
        /// 分类
        /// </summary>
        public CategoryInfo Category { get; set; }
        /// <summary>
        /// post's category selectitem
        /// </summary>
        public List<SelectListItem> CateSelectItem { get; set; }

        public List<CategoryInfo> CateList { get; set; }
        /// <summary>
        /// 页面类型
        /// </summary>
        public List<SelectListItem> CateType { get; set; }
    }
}