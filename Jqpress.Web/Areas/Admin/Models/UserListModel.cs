using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Core.Domain;


namespace Jqpress.Web.Areas.Admin.Models
{
    public class UserListModel
    {
        public UserListModel()
        {
            UserList = new List<UserInfo>();
            RolesCateItem = new List<SelectListItem>();
        }
        /// <summary>
        /// user list
        /// </summary>
        public List<UserInfo> UserList { get; set; }
        /// <summary>
        /// roles list
        /// </summary>
        public List<SelectListItem> RolesCateItem { get; set; }
    
    }
}