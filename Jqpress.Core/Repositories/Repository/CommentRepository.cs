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
    public partial class CommentRepository:ICommentRepository
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public int Insert(CommentInfo comment)
        {
            string cmdText = string.Format(@"insert into [{0}comments](
                            PostId, ParentId,UserId,Author,Email,AuthorUrl,Contents,EmailNotify,IpAddress,CreateTime,Approved)
                             values (
                            @PostId, @ParentId,@UserId,@Author,@Email,@AuthorUrl,@Contents,@EmailNotify,@IpAddress,@CreateTime,@Approved)", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                conn.Execute(cmdText, comment);
                //return conn.Query<int>(string.Format("select top 1 [CommentId] from [{0}comments]  order by [CommentId] desc", ConfigHelper.Tableprefix), null).First();
                return conn.Query<int>(string.Format("select  [CommentId] from [{0}comments]  order by [CommentId] desc limit 1", ConfigHelper.Tableprefix), null).First();//for sqlite
            }

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public int Update(CommentInfo comment)
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
                            where CommentId=@CommentId ", ConfigHelper.Tableprefix);

            using (var conn = new DapperHelper().OpenConnection())
            {
               return conn.Execute(cmdText, comment);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public int Delete(CommentInfo comment)
        {
            string cmdText = string.Format("delete from [{0}comments] where [commentId] = @commentId", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new { commentId = comment.CommentId });
            }
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CommentInfo GetById(object id)
        {
            string cmdText = string.Format("select * from [{0}comments] where [commentId] = @commentId", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<CommentInfo>(cmdText, new { commentId = (int)id });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }
        /// <summary>
        /// 获取所有评论
        /// </summary>
        public virtual IEnumerable<CommentInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}comments] order by creattime desc", ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<CommentInfo>(cmdText, null);
                    return list;
                }
            }
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

           
            using (var conn = new DapperHelper().OpenConnection())
            {
                string cmdTotalRecord = "select count(1) from [" + ConfigHelper.Tableprefix + "comments] where " + condition;
                totalRecord = conn.Query<int>(cmdTotalRecord, null).First();
                string cmdText = new DapperHelper().GetPageSql("[" + ConfigHelper.Tableprefix + "comments]", "[CommentId]", "*", pageSize, pageIndex, 1, condition);

                var list = conn.Query<CommentInfo>(cmdText, null);
                return list.ToList();
            }

        }

        /// <summary>
        /// 根据日志ID删除评论
        /// </summary>
        /// <param name="postId">日志ID</param>
        /// <returns></returns>
        public int DeleteCommentByPost(int postId)
        {
            string cmdText = string.Format("delete from [{0}comments] where [postId] = @postId", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new { postId = postId });
            }
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
            using (var conn = new DapperHelper().OpenConnection()) 
            {
                string cmdTotalRecord = "select count(1) from [" + ConfigHelper.Tableprefix + "comments] where " + condition;
                return conn.Query<int>(cmdTotalRecord, null).First();
            }
            
        }
    }
}
