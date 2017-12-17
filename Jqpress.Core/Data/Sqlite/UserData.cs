using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using Jqpress.Blog.Entity;
using Jqpress.Framework.DbProvider.Access;
using Jqpress.Framework.Configuration;

namespace Jqpress.Blog.Data.Sqlite
{
    public partial class DataProvider
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public int InsertUser(UserInfo userinfo)
        {
            string cmdText =string.Format(@" insert into [{0}users](
                                [UserType],[UserName],[NickName],[Password],[Email],[SiteUrl],[AvatarUrl],[Description],[sortnum],[Status],[PostCount],[CommentCount],[CreateTime])
                                values (
                                @UserType,@UserName,@NickName,@Password,@Email,@SiteUrl,@AvatarUrl,@Description,@SortNum,@Status, @PostCount,@CommentCount,@CreateTime )",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                        OleDbHelper.MakeInParam("@UserType", OleDbType.Integer,4, userinfo.UserType),
                                        OleDbHelper.MakeInParam("@UserName", OleDbType.VarWChar,50, userinfo.UserName),
                                        OleDbHelper.MakeInParam("@NickName", OleDbType.VarWChar,50, userinfo.NickName),
                                        OleDbHelper.MakeInParam("@Password", OleDbType.VarWChar,50, userinfo.Password),
                                        OleDbHelper.MakeInParam("@Email", OleDbType.VarWChar,50, userinfo.Email),
                                        OleDbHelper.MakeInParam("@SiteUrl", OleDbType.VarWChar,255, userinfo.SiteUrl),
                                        OleDbHelper.MakeInParam("@AvatarUrl", OleDbType.VarWChar,255, userinfo.AvatarUrl),
                                        OleDbHelper.MakeInParam("@Description", OleDbType.VarWChar,255, userinfo.Description),
                                        OleDbHelper.MakeInParam("@SortNum", OleDbType.Integer,4, userinfo.SortNum),
                                        OleDbHelper.MakeInParam("@Status", OleDbType.Integer,4, userinfo.Status),                           
                                        OleDbHelper.MakeInParam("@PostCount", OleDbType.Integer,4, userinfo.PostCount),
                                        OleDbHelper.MakeInParam("@CommentCount", OleDbType.Integer,4, userinfo.CommentCount),
                                        OleDbHelper.MakeInParam("@CreateTime", OleDbType.Date,8, userinfo.CreateTime),
                                        
                                    };
            int r = OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
            if (r > 0)
            {
                return Convert.ToInt32(OleDbHelper.ExecuteScalar(string.Format("select top 1 [UserId] from [{0}users]  order by [UserId] desc",ConfigHelper.Tableprefix)));
            }
            return 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public int UpdateUser(UserInfo userinfo)
        {
            string cmdText =string.Format( @"update [{0}users] set
                                [UserType]=@UserType,
                                [UserName]=@UserName,
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
                                where UserId=@UserId",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                        OleDbHelper.MakeInParam("@UserType", OleDbType.Integer,4, userinfo.UserType),
                                        OleDbHelper.MakeInParam("@UserName", OleDbType.VarWChar,50, userinfo.UserName),
                                        OleDbHelper.MakeInParam("@NickName", OleDbType.VarWChar,50, userinfo.NickName),
                                        OleDbHelper.MakeInParam("@Password", OleDbType.VarWChar,50, userinfo.Password),
                                        OleDbHelper.MakeInParam("@Email", OleDbType.VarWChar,50, userinfo.Email),
                                        OleDbHelper.MakeInParam("@SiteUrl", OleDbType.VarWChar,255, userinfo.SiteUrl),
                                        OleDbHelper.MakeInParam("@AvatarUrl", OleDbType.VarWChar,255, userinfo.AvatarUrl),
                                        OleDbHelper.MakeInParam("@Description", OleDbType.VarWChar,255, userinfo.Description),
                                        OleDbHelper.MakeInParam("@SortNum", OleDbType.VarWChar,255, userinfo.SortNum),
                                        OleDbHelper.MakeInParam("@Status", OleDbType.Integer,4, userinfo.Status),                           
                                        OleDbHelper.MakeInParam("@PostCount", OleDbType.Integer,4, userinfo.PostCount),
                                        OleDbHelper.MakeInParam("@CommentCount", OleDbType.Integer,4, userinfo.CommentCount),
                                        OleDbHelper.MakeInParam("@CreateTime", OleDbType.Date,8, userinfo.CreateTime),
                                        OleDbHelper.MakeInParam("@UserId", OleDbType.Integer,4, userinfo.UserId),
                                    };
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int DeleteUser(int userid)
        {
            string cmdText = string.Format("delete from [{0}users] where [userid] = @userid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								        OleDbHelper.MakeInParam("@userid",OleDbType.Integer,4,userid)
							        };
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetUserList()
        {
            string cmdText = string.Format("select * from [{0}users]  order by [sortnum] asc,[userid] asc",ConfigHelper.Tableprefix);
            return DataReaderToUserList(OleDbHelper.ExecuteReader(cmdText));

        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        private static List<UserInfo> DataReaderToUserList(OleDbDataReader read)
        {
            var list = new List<UserInfo>();
            while (read.Read())
            {
                var userinfo = new UserInfo
                                   {
                                       UserId = Convert.ToInt32(read["UserId"]),
                                       UserType = Convert.ToInt32(read["UserType"]),
                                       UserName = Convert.ToString(read["UserName"]),
                                       NickName = Convert.ToString(read["NickName"]),
                                       Password = Convert.ToString(read["Password"]),
                                       Email = Convert.ToString(read["Email"]),
                                       SiteUrl = Convert.ToString(read["SiteUrl"]),
                                       AvatarUrl = Convert.ToString(read["AvatarUrl"]),
                                       Description = Convert.ToString(read["Description"]),
                                       SortNum = Convert.ToInt32(read["SortNum"]),
                                       Status = Convert.ToInt32(read["Status"]),
                                       PostCount = Convert.ToInt32(read["PostCount"]),
                                       CommentCount = Convert.ToInt32(read["CommentCount"]),
                                       CreateTime = Convert.ToDateTime(read["CreateTime"])
                                   };


                list.Add(userinfo);
            }
            read.Close();
            return list;
        }


        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool ExistsUserName(string userName)
        {
            string cmdText = string.Format("select count(1) from [{0}users] where [userName] = @userName ",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                        OleDbHelper.MakeInParam("@userName",OleDbType.VarWChar,50,userName),
							        };
            return Convert.ToInt32(OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams)) > 0;
        }
    }
}
