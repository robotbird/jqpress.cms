using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class TagListModel : BaseModel
    {
        public TagListModel() 
        {
            TagList = new List<TagInfo>();
            PageList = new TagPageList();
        }
        
        public List<TagInfo> TagList { get; set; }

        public TagPageList PageList { get; set; }
        public class TagPageList : BasePageableModel { };

    }
}