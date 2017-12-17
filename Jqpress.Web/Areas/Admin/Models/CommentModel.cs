using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class CommentModel : BaseModel
    {
        public CommentModel() 
        {
            CommentList = new List<CommentInfo>();
            PageList = new CommentPageList();
        }

        public List<CommentInfo> CommentList { get; set; }

        public CommentPageList PageList { get; set; }
        public class CommentPageList : BasePageableModel { };

    }
}