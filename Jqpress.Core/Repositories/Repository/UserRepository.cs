using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.DbProvider;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Repositories.Repository
{
    public partial class UserRepository:IUserRepository
    {
       

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public int Insert(UserInfo userinfo)
        {
            string cmdText = string.Format(@" insert into [{0}users](
                                [Role],[UserName],[NickName],[Password],[Email],[SiteUrl],[AvatarUrl],[Description],[sortnum],[Status],[PostCount],[CommentCount],[CreateTime])
                                values (
                                @Role,@UserName,@NickName,@Password,@Email,@SiteUrl,@AvatarUrl,@Description,@SortNum,@Status, @PostCount,@CommentCount,@CreateTime )", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                conn.Execute(cmdText, userinfo);
                //return conn.Query<int>(string.Format("select top 1 [UserId] from [{0}users]  order by [UserId] desc", ConfigHelper.Tableprefix), null).First();
                return conn.Query<int>(string.Format("select  [UserId] from [{0}users]  order by [UserId] desc limit 1", ConfigHelper.Tableprefix), null).First();//for sqlite
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public int Update(UserInfo userinfo)
        {
            string cmdText = string.Format(@"update [{0}users] set
                                [Role]=@Role,
                                [NickName]=@NickName,
                                [Password]=@Password,
                                [Email]=@Email,
                                [SiteUrl]=@SiteUrl,
                                [AvatarUrl]=@AvatarUrl,
                                [Description]=@Description,
                                [SortNum]=@SortNum,
                                [Status]=@Status,
                                [PostCount]=@PostCount,
                                [CommentCount]=@CommentCount,
                                [CreateTime]=@CreateTime
                                where UserId=@UserId", ConfigHelper.Tableprefix);
           using(var conn = new DapperHelper().OpenConnection())
           {
              return conn.Execute(cmdText, userinfo);
           }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int Delete(UserInfo userinfo)
        {
            string cmdText = string.Format("delete from [{0}users] where [userid] = @userid", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new {userid = userinfo.UserId });
            }
        }


        /// <summary>
        /// 获取用户 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public UserInfo GetById(object Id)
        {
            string cmdText = string.Format("select * from [{0}users] where [UserId] = @userId ", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<UserInfo>(cmdText, new { userId = (int)Id });
                return list.ToList().Count > 0 ? list.First() : null;
            }
        }

        /// <summary>
        /// 根据用户名获取用户 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserInfo GetUserByName(string userName) 
        {
            string cmdText = string.Format("select * from [{0}users] where [userName] = @userName ", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<UserInfo>(cmdText, new {userName = userName});
                if (list.Any())
                {
                    return list.First();
                }
                return null;
            }
        }

        /// <summary>
        /// 根据用户名和密码获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserInfo GetUser(string userName, string password) 
        {
            string cmdText = string.Format("select * from [{0}users] where [userName] = @userName and [password]=@password ", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list =conn.Query<UserInfo>(cmdText, new { userName = userName,password = password });
                return list.Count()==0?null:list.First();
            }
        }

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<UserInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}users]  order by [sortnum] asc,[userid] asc", ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<UserInfo>(cmdText, null);
                    return list;
                }
            }
        }



        /// <summary>
        /// 是否存在此用户名
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool ExistsUserName(string userName)
        {
            string cmdText = string.Format("select count(1) from [{0}users] where [userName] = @userName ", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new {userName = userName })>0;
            }
        }
    }
}
