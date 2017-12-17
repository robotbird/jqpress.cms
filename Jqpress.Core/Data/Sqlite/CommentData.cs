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
        /// <param name="comment"></param>
        /// <returns></returns>
        public int InsertComment(CommentInfo comment)
        {
            string cmdText = string.Format(@"insert into [{0}comments](
                            PostId, ParentId,UserId,Author,Email,AuthorUrl,Contents,EmailNotify,IpAddress,CreateTime,Approved)
                             values (
                            @PostId, @ParentId,@UserId,@Author,@Email,@AuthorUrl,@Contents,@EmailNotify,@IpAddress,@CreateTime,@Approved)",ConfigHelper.Tableprefix);

            OleDbParameter[] prams = { 
                                        OleDbHelper.MakeInParam("@PostId", OleDbType.Integer,4, comment.PostId),
                                        OleDbHelper.MakeInParam("@ParentId", OleDbType.Integer,4, comment.ParentId),
                                        OleDbHelper.MakeInParam("@UserId", OleDbType.Integer,4, comment.UserId),
                                        OleDbHelper.MakeInParam("@Author", OleDbType.VarWChar,255, comment.Author),
                                        OleDbHelper.MakeInParam("@Email", OleDbType.VarWChar,255, comment.Email),
                                        OleDbHelper.MakeInParam("@AuthorUrl", OleDbType.VarWChar,255, comment.AuthorUrl),
                                        OleDbHelper.MakeInParam("@Contents", OleDbType.VarWChar,255, comment.Contents),
                                        OleDbHelper.MakeInParam("@EmailNotify", OleDbType.Integer,4 ,    comment.EmailNotify),
                                        OleDbHelper.MakeInParam("@IpAddress", OleDbType.VarWChar,255, comment.IpAddress),
                                        OleDbHelper.MakeInParam("@CreateTime", OleDbType.Date,8, comment.CreateTime),
                                        OleDbHelper.MakeInParam("@Approved", OleDbType.Integer,4 ,   comment.Approved),
            };
            OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);

            int newId = Convert.ToInt32(OleDbHelper.ExecuteScalar(string.Format("select top 1 [CommentId] from [{0}comments]  order by [CommentId] desc",ConfigHelper.Tableprefix)));
            return newId;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public int UpdateComment(CommentInfo comment)
        {
            string cmdText = string.Format(@"update [{0}comments] set 
                            PostId=@PostId,                            
                            ParentId=@ParentId,
                            UserId=@UserId,
                            Author=@Author,
                            Email=@Email,
                            AuthorUrl=@AuthorUrl,
                            Contents=@Contents,
                            EmailNotify=@EmailNotify,
                            IpAddress=@IpAddress,
                            CreateTime=@CreateTime,
                            Approved=@Approved
                            where CommentId=@CommentId ",ConfigHelper.Tableprefix);

            OleDbParameter[] prams = { 
                                        OleDbHelper.MakeInParam("@PostId", OleDbType.Integer,4, comment.PostId),
                                        OleDbHelper.MakeInParam("@ParentId", OleDbType.Integer,4, comment.ParentId),
                                        OleDbHelper.MakeInParam("@UserId", OleDbType.Integer,4, comment.UserId),
                                        OleDbHelper.MakeInParam("@Author", OleDbType.VarWChar,255, comment.Author),
                                        OleDbHelper.MakeInParam("@Email", OleDbType.VarWChar,255, comment.Email),
                                        OleDbHelper.MakeInParam("@AuthorUrl", OleDbType.VarWChar,255, comment.AuthorUrl),
                                        OleDbHelper.MakeInParam("@Contents", OleDbType.VarWChar,255, comment.Contents),
                                        OleDbHelper.MakeInParam("@EmailNotify", OleDbType.Integer,4 ,    comment.EmailNotify),
                                        OleDbHelper.MakeInParam("@IpAddress", OleDbType.VarWChar,255, comment.IpAddress),
                                        OleDbHelper.MakeInParam("@CreateTime", OleDbType.Date,8, comment.CreateTime),
                                        OleDbHelper.MakeInParam("@Approved", OleDbType.Integer,4 ,   comment.Approved),
                                        OleDbHelper.MakeInParam("@CommentId", OleDbType.Integer,4, comment.CommentId),

                                    };
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public int DeleteComment(int commentId)
        {

            string cmdText = string.Format("delete from [{0}comments] where [commentId] = @commentId",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@commentId",OleDbType.Integer,4,commentId)
							};

            int result = OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public CommentInfo GetComment(int commentId)
        {
            string cmdText = string.Format("select * from [{0}comments] where [commentId] = @commentId",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								        OleDbHelper.MakeInParam("@commentId",OleDbType.Integer,4,commentId)
							          };
            List<CommentInfo> list = DataReaderToCommentList(OleDbHelper.ExecuteReader(cmdText, prams));

            return list.Count > 0 ? list[0] : null;
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalRecord"></param>
        /// <param name="order"></param>
        /// <param name="parentId"></param>
        /// <param name="approved"></param>
        /// <param name="emailNotify"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<CommentInfo> GetCommentList(int pageSize, int pageIndex, out int totalRecord, int order, int userId, int postId, int parentId, int approved, int emailNotify, string keyword)
        {
            string condition = " 1=1 ";// "[ParentId]=0 and [PostId]=" + postId;

            if (userId != -1)
            {
                condition += " and userid=" + userId;
            }
            if (postId != -1)
            {
                condition += " and postId=" + postId;
            }
            if (parentId != -1)
            {
                condition += " and parentId=" + parentId;
            }

            if (approved != -1)
            {
                condition += " and approved=" + approved;
            }

            if (emailNotify != -1)
            {
                condition += " and emailNotify=" + emailNotify;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                condition += string.Format(" and (content like '%{0}%' or author like '%{0}%' or ipaddress like '%{0}%' or email like '%{0}%'  or siteurl like '%{0}%' )", keyword);
            }

            string cmdTotalRecord = "select count(1) from ["+ConfigHelper.Tableprefix+"comments] where " + condition;
            totalRecord = Convert.ToInt32(OleDbHelper.ExecuteScalar(CommandType.Text, cmdTotalRecord));

            //   throw new Exception(cmdTotalRecord);

            string cmdText = OleDbHelper.GetPageSql("["+ConfigHelper.Tableprefix+"comments]", "[CommentId]", "*", pageSize, pageIndex, order, condition);
            return DataReaderToCommentList(OleDbHelper.ExecuteReader(cmdText));
        }

        /// <summary>
        /// 根据日志ID删除评论
        /// </summary>
        /// <param name="postId">日志ID</param>
        /// <returns></returns>
        public int DeleteCommentByPost(int postId)
        {
            string cmdText = string.Format("delete from [{0}comments] where [postId] = @postId",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@postId",OleDbType.Integer,4,postId)
							};
            int result = OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);

            return result;
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        private static List<CommentInfo> DataReaderToCommentList(OleDbDataReader read)
        {
            var list = new List<CommentInfo>();
            while (read.Read())
            {
                var comment = new CommentInfo
                                  {
                                      CommentId = Convert.ToInt32(read["CommentId"]),
                                      ParentId = Convert.ToInt32(read["ParentId"]),
                                      PostId = Convert.ToInt32(read["PostId"]),
                                      UserId = Convert.ToInt32(read["UserId"]),
                                      Author = Convert.ToString(read["Author"]),
                                      Email = Convert.ToString(read["Email"]),
                                      AuthorUrl = Convert.ToString(read["AuthorUrl"]),
                                      Contents = Convert.ToString(read["Contents"]),
                                      EmailNotify = Convert.ToInt32(read["EmailNotify"]),
                                      IpAddress = Convert.ToString(read["IpAddress"]),
                                      CreateTime = Convert.ToDateTime(read["CreateTime"]),
                                      Approved = Convert.ToInt32(read["Approved"])
                                  };
                list.Add(comment);
            }
            read.Close();
            return list;
        }


        /// <summary>
        /// 统计评论
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="incChild"></param>
        /// <returns></returns>
        public int GetCommentCount(int userId, int postId, bool incChild)
        {
            string condition = " 1=1 ";
            if (userId != -1)
            {
                condition += " and [userId] = " + userId;
            }
            if (postId != -1)
            {
                condition += " and [postId] = " + postId;
            }
            if (incChild == false)
            {
                condition += " and [parentid]=0";
            }
            string cmdText = string.Format("select count(1) from [{0}comments] where " + condition,ConfigHelper.Tableprefix);
            return Convert.ToInt32(OleDbHelper.ExecuteScalar(CommandType.Text, cmdText));
        }
    }
}
