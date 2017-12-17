using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Repositories.IRepository;
using Jqpress.Core.Domain;

namespace Jqpress.Core.Repositories.IRepository
{
   public partial  interface IUserRepository:IRepository<UserInfo>
    {
        /// <summary>
        /// 是否存在此用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool ExistsUserName(string userName);

        /// <summary>
        /// 根据用户名获取用户 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        UserInfo GetUserByName(string userName);
        /// <summary>
        /// 根据用户名和密码获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserInfo GetUser(string userName, string password);
    }
}
