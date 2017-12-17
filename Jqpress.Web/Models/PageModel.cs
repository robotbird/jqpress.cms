using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Models
{
    public class PageModel:BaseModel
    {
        public PageModel() 
        {
            category = new CategoryInfo();
        }
        /// <summary>
        /// 分类
        /// </summary>
        public CategoryInfo category { get; set; }
    }
}