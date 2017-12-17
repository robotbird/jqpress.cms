using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Services;


namespace Jqpress.Core.Common
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperateType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Insert = 0,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
    }
    /// <summary>
    /// 后台基类
    /// </summary>
    public class AdminPage : BasePage
    {
         protected SiteConfigInfo setting;

        public AdminPage()
        {
            CheckLoginAndPermission();
            setting = Jqpress.Core.Configuration.SiteConfig.GetSetting();
        }

        /// <summary>
        /// 检查登录和权限
        /// </summary>
        protected void CheckLoginAndPermission()
        {
            if (!IsLogin)
            {
                HttpContext.Current.Response.Redirect("/admin/login?returnurl=" + HttpContext.Current.Server.UrlEncode(Jqpress.Framework.Web.PressRequest.CurrentUrl));
                HttpContext.Current.Response.End();
            }
            else
            {
                UserInfo user = new UserService().GetUser(CurrentUserId);

                if (user == null)       //删除已登陆用户时有效
                {
                    RemoveUserCookie();
                    HttpContext.Current.Response.Redirect("/admin/login?returnurl=" + HttpContext.Current.Server.UrlEncode(Jqpress.Framework.Web.PressRequest.CurrentUrl));

                }

                if (Jqpress.Framework.Utils.EncryptHelper.MD5(user.UserId + HttpContext.Current.Server.UrlEncode(user.UserName) + user.Password) != CurrentKey)
                {
                    RemoveUserCookie();
                    HttpContext.Current.Response.Redirect("/admin/login?returnurl=" + HttpContext.Current.Server.UrlEncode(Jqpress.Framework.Web.PressRequest.CurrentUrl));
                }

                if (CurrentUser.Status == 0)
                {
                    ResponseError("您的用户名已停用", "您的用户名已停用,请与管理员联系!");
                }

                string[] plist = new string[] { "theme/list", "theme/edit", "link/list", "user/list", "setting", "category/list", "tag/list", "comment" };
                if (CurrentUser.Role == (int)UserRole.Author)
                {
                    string pageName = System.IO.Path.GetFileName(HttpContext.Current.Request.Url.ToString()).ToLower();

                    foreach (string p in plist)
                    {
                        if (pageName == p)
                        {
                            ResponseError("没有权限", "您没有权限使用此功能,请与管理员联系!");
                        }
                    }
                }

            }



        }



        protected void SetPageTitle(string title)
        {
            Page.Title = title + " - 管理中心 - Powered by Jqpress";
        }


        /// <summary>
        /// 操作
        /// </summary>
        private string _operate = Jqpress.Framework.Web.PressRequest.GetQueryString("Operate");
        
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperateType Operate
        {
            get
            {
                switch (_operate)
                {
                    case "update":
                        return OperateType.Update;
                    case "delete":
                        return OperateType.Delete;
                    default:
                        return OperateType.Insert;
                }
            }
        }

        /// <summary>
        /// 操作字符串
        /// </summary>
        public string OperateString
        {
            get
            {
                return _operate;
            }
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        protected string ResponseMessage = string.Empty;

        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string ShowError(string error)
        {
            ResponseMessage = "<div class=\"p_error\">";
            ResponseMessage += error;
            ResponseMessage += "</div>";
            return ResponseMessage;
        }

        /// <summary>
        /// 正确提示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string ShowMessage(string message)
        {

            ResponseMessage = "<div class=\"p_message\">";
            ResponseMessage += message;
            ResponseMessage += "</div>";
            return ResponseMessage;
        }
    }
}
