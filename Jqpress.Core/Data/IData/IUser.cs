using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;

namespace Jqpress.Blog.Data.IData
{
    partial interface IDataProvider
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="_userinfo"></param>
        /// <returns></returns>
        int InsertUser(UserInfo _userinfo);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="_userinfo"></param>
        /// <returns></returns>
        int UpdateUser(UserInfo _userinfo);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        int DeleteUser(int userID);

        ///// <summary>
        ///// 根据用户名和密码获取用户
        ///// </summary>
        ///// <param name="userName"></param>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //UserInfo GetUser(string userName, string password);

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        List<UserInfo> GetUserList();

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool ExistsUserName(string userName);

    }
}
