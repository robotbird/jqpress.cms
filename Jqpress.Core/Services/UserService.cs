using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jqpress.Core.Common;
using Jqpress.Core.Domain;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Repositories.Repository;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Services
{
   public class UserService
    {
        private IUserRepository _userRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public UserService()
            : this(new UserRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }
        #endregion


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="_userinfo"></param>
        /// <returns></returns>
        public  int InsertUser(UserInfo _userinfo)
        {
            _userinfo.UserId = _userRepository.Insert(_userinfo);
            return _userinfo.UserId;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="_userinfo"></param>
        /// <returns></returns>
        public  int UpdateUser(UserInfo _userinfo)
        {
            return _userRepository.Update(_userinfo);
        }

        /// <summary>
        /// 更新用户文章数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public  int UpdateUserPostCount(int userId, int addCount)
        {
            UserInfo user = GetUser(userId);
            if (user != null)
            {
                user.PostCount += addCount;
                return UpdateUser(user);
            }
            return 0;
        }

        /// <summary>
        /// 更新用户评论数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public  int UpdateUserCommentCount(int userId, int addCount)
        {
            UserInfo user = GetUser(userId);
            if (user != null)
            {
                user.CommentCount += addCount;

                return UpdateUser(user);
            }
            return 0;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public  int DeleteUser(int userId)
        {
            return _userRepository.Delete(new UserInfo { UserId= userId });
        }


        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        public  List<UserInfo> GetUserList()
        {
            return _userRepository.Table.ToList();
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public  bool ExistsUserName(string userName)
        {
            return _userRepository.ExistsUserName(userName);
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public  UserInfo GetUser(int userId)
        {
            return _userRepository.GetById(userId);
        }

        /// <summary>
        /// 根据用户名获取用户 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public  UserInfo GetUser(string userName)
        {

            return _userRepository.GetUserByName(userName);
        }

        /// <summary>
        /// 根据用户名和密码获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public  UserInfo GetUser(string userName, string password)
        {
            return _userRepository.GetUser(userName, password);
        }

        #region 用户COOKIE操作
        /// <summary>
        /// 用户COOKIE名
        /// </summary>
        private  readonly string CookieUser = ConfigHelper.SitePrefix + "admin";
        /// <summary>
        /// 读当前用户COOKIE
        /// </summary>
        /// <returns></returns>
        public  HttpCookie ReadUserCookie()
        {
            return HttpContext.Current.Request.Cookies[CookieUser];
        }

        /// <summary>
        /// 移除当前用户COOKIE
        /// </summary>
        /// <returns></returns>
        public  bool RemoveUserCookie()
        {
            HttpCookie cookie = new HttpCookie(CookieUser);
            cookie.Values.Clear();
            cookie.Expires = DateTime.Now.AddYears(-1);

            HttpContext.Current.Response.AppendCookie(cookie);
            return true;
        }

        /// <summary>
        /// 写/改当前用户COOKIE
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public  bool WriteUserCookie(int userID, string userName, string password, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieUser];
            if (cookie == null)
            {
                cookie = new HttpCookie(CookieUser);
            }
            if (expires > 0)
            {
                cookie.Values["expires"] = expires.ToString();
                cookie.Expires = DateTime.Now.AddMinutes(expires);
            }
            else
            {
                int temp_expires = Convert.ToInt32(cookie.Values["expires"]);
                if (temp_expires > 0)
                {
                    cookie.Expires = DateTime.Now.AddMinutes(temp_expires);
                }
            }
            cookie.Values["userid"] = userID.ToString();
            cookie.Values["username"] = HttpContext.Current.Server.UrlEncode(userName);
            cookie.Values["key"] = Jqpress.Framework.Utils.EncryptHelper.MD5(userID + HttpContext.Current.Server.UrlEncode(userName) + password);

            HttpContext.Current.Response.AppendCookie(cookie);
            return true;
        }
        #endregion
    }
}
