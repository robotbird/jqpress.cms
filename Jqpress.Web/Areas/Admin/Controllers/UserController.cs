using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Web;
using Jqpress.Framework.Utils;
using Jqpress.Core.Services;
using Jqpress.Core.Domain;
using Jqpress.Web.Areas.Admin.Models;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        #region private items
        private UserService _userService = new UserService();
        #endregion;
        public ActionResult List()
        {
            var model = new UserListModel();


            List<UserInfo> list = _userService.GetUserList();
            model.UserList = list;
           

            return View(model);
        }

        /// <summary>
        /// get user by id
        /// </summary>
        public ActionResult Edit(int? id)
        {
            var uid = id ?? 0;
            var model =new UserModel();

            model.RolesCateItem = LoadRole(0);
            if (uid > 0) 
            {
                model.user = _userService.GetUser(uid);
                model.RolesCateItem = LoadRole(model.user.Role);
            }
            return View(model);
        }
        private List<SelectListItem> LoadRole(int id) 
        {
            var dic = new Dictionary<string, string>();
            dic.Add("1", "管理员");
            dic.Add("2", "编辑");
            dic.Add("3", "订阅者");
            dic.Add("4", "投稿者");
            dic.Add("5", "作者");
            var list = new List<SelectListItem>();
            foreach (int role in Enum.GetValues(typeof(Jqpress.Core.Domain.Enum.UserRole)))
            {
                string name = Enum.GetName(typeof(Jqpress.Core.Domain.Enum.UserRole), role);
                foreach (KeyValuePair<string, string> kv in dic)
                {
                    if (id == Convert.ToInt32(kv.Key))
                    {
                        var Item = new SelectListItem() { Text = kv.Value, Value = role.ToString(),Selected =true };
                        list.Add(Item);
                    }
                    else 
                    {
                        var Item = new SelectListItem() { Text = kv.Value, Value = role.ToString() };
                        list.Add(Item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// delete user by id
        /// </summary>
        public ActionResult Delete(int? id)
        {
            var uid = id ?? 0;
            _userService.DeleteUser(uid);
            SuccessNotification("删除成功");
            return RedirectToAction("list");
        }
        /// <summary>
        /// insert or update user
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Save"), ValidateInput(false)]
        public ActionResult Save(UserInfo u)
        {
            var password2 = Request.Form["password2"];

            var flag = true;
            if (!string.IsNullOrEmpty(u.Password) && u.Password != password2)
            {
                ErrorNotification("两次密码输入不相同!");
                flag = false;
            }
            else 
            {
                if (!string.IsNullOrEmpty(u.Password)) 
                {
                    u.Password = EncryptHelper.MD5(u.Password);                
                }
            }

            if (u.UserId > 0)
            {
                var olduser = _userService.GetUser(u.UserId);
                u.CreateTime = olduser.CreateTime;
                u.CommentCount = olduser.CommentCount;
                u.PostCount = olduser.PostCount;
                if (string.IsNullOrEmpty(u.Password)) 
                {
                    u.Password = olduser.Password;
                }
            }
            else
            {
                u.CommentCount = 0;
                u.CreateTime = DateTime.Now;
                u.PostCount = 0;
            }
            u.SiteUrl = string.Empty;
            u.AvatarUrl = string.Empty;
            u.Description = string.Empty;
            u.Status = 1;


            #region 验证处理
            if (string.IsNullOrEmpty(u.UserName))
            {
                ErrorNotification("请输入登陆用户名!");
                flag = false;
            }

            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[A-Za-z0-9\u4e00-\u9fa5-]");
            if (!reg.IsMatch(u.UserName))
            {
                ErrorNotification("用户名限字母,数字,中文,连字符!");
                flag = false;
            }
            #endregion

            if (u.UserId>0)//更新操作
            {
                if (flag)
                {
                    _userService.UpdateUser(u);
                    //  如果修改当前用户,则更新COOKIE
                    if (!string.IsNullOrEmpty(u.Password) && u.UserId == CurrentUserId)
                    {
                       _userService.WriteUserCookie(u.UserId, u.UserName, u.Password, 0);
                    }
                    SuccessNotification("修改成功");
                    return RedirectToAction("list");
                }
                else 
                {
                    return RedirectToAction("edit", new { id=u.UserId});                   
                }
            }
            else//添加操作
            {

                if (string.IsNullOrEmpty(u.Password))
                {
                    ErrorNotification("请输入密码!");
                    flag = false;
                }
                if (_userService.ExistsUserName(u.UserName))
                {
                    ErrorNotification("该用户名已存在,请换之");
                    flag = false;
                }
                if (flag)
                {
                    u.UserId = _userService.InsertUser(u);
                    SuccessNotification("添加成功");
                    return RedirectToAction("list");
                }
                else 
                {
                    return RedirectToAction("edit");
                }

            }
        }
    }
}
