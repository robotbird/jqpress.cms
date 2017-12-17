using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class UserModel
    {
        public UserModel() 
        {
            user = new UserInfo();
            RolesCateItem = new List<SelectListItem>();
        }
        public UserInfo user { get; set; }
        /// <summary>
        /// roles list
        /// </summary>
        public List<SelectListItem> RolesCateItem { get; set; }
    }
}