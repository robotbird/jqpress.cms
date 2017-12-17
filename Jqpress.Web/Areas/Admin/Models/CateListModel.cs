using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class CateListModel
    {
        public CateListModel() 
        {
            CateSelectItem = new List<SelectListItem>();
            CateList = new List<CategoryInfo>();
        }
        /// <summary>
        /// post's category selectitem
        /// </summary>
        public List<SelectListItem> CateSelectItem { get; set; }

        public List<CategoryInfo> CateList { get; set; }
    }
}