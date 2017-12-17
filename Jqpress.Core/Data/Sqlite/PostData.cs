using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using Jqpress.Blog.Data.IData;
using Jqpress.Blog.Entity;
using Jqpress.Framework.DbProvider.Access;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.DbProvider;

namespace Jqpress.Blog.Data.Sqlite
{
    public partial class DataProvider : IDataProvider
    {
        DapperHelper dapper = new DapperHelper();
        /// <summary>
        /// 获取全部列表
        /// </summary>
        /// <returns></returns>
        public List<PostInfo> GetPostList()
        {
            string cmdText = string.Format("select * from [{0}posts] order by [postid] desc", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                var list = conn.Query<PostInfo>(cmdText, null);
                return list.ToList();
            }
        }

        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <returns></returns>
        private static void CheckSlug(PostInfo post)
        {
            if (string.IsNullOrEmpty(post.Slug))
            {
                return;
            }
            while (true)
            {
                string cmdText = post.PostId == 0 ? string.Format("select count(1) from [{1}posts] where [slug]='{0}'  ", post.Slug,ConfigHelper.Tableprefix) : string.Format("select count(1) from [{2}posts] where [slug]='{0}'   and [postid]<>{1}", post.Slug, post.PostId, ConfigHelper.Tableprefix);
                int r = Convert.ToInt32(OleDbHelper.ExecuteScalar(cmdText));
                if (r == 0)
                {
                    return;
                }
                post.Slug += "-2";
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="postinfo">实体</param>
        /// <returns>成功返回新记录的ID,失败返回 0</returns>
        public int InsertPost(PostInfo postinfo)
        {
            CheckSlug(postinfo);
            string cmdText = string.Format(@"insert into [{0}posts]
                                (
                               [CategoryId],[Title],[Summary],[PostContent],[Slug],[UserId],[CommentStatus],[CommentCount],[ViewCount],[Tag],[UrlFormat],[Template],[Recommend],[Status],[TopStatus],[HomeStatus],[HideStatus],[PostTime],[UpdateTime]
                                )
                                values
                                (
                                @CategoryId,@Title,@Summary,@PostContent,@Slug,@UserId,@CommentStatus,@CommentCount,@ViewCount,@Tag,@UrlFormat,@Template,@Recommend,@Status,@TopStatus,@HomeStatus,@HideStatus,@PostTime,@UpdateTime
                                )",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								
                                OleDbHelper.MakeInParam("@CategoryId",OleDbType.Integer,4,postinfo.CategoryId),
								OleDbHelper.MakeInParam("@Title",OleDbType.VarWChar,255,postinfo.Title),
								OleDbHelper.MakeInParam("@Summary",OleDbType.VarWChar,0,postinfo.Summary),
								OleDbHelper.MakeInParam("@PostContent",OleDbType.VarWChar,0,postinfo.PostContent),
								OleDbHelper.MakeInParam("@Slug",OleDbType.VarWChar,255,postinfo.Slug),
								OleDbHelper.MakeInParam("@UserId",OleDbType.Integer,4,postinfo.UserId),
								OleDbHelper.MakeInParam("@CommentStatus",OleDbType.Integer,1,postinfo.CommentStatus),
								OleDbHelper.MakeInParam("@CommentCount",OleDbType.Integer,4,postinfo.CommentCount),
								OleDbHelper.MakeInParam("@ViewCount",OleDbType.Integer,4,postinfo.ViewCount),
								OleDbHelper.MakeInParam("@Tag",OleDbType.VarWChar,255,postinfo.Tag),
                                OleDbHelper.MakeInParam("@UrlFormat",OleDbType.Integer,1,postinfo.UrlFormat),
                                OleDbHelper.MakeInParam("@Template",OleDbType.VarChar,50,postinfo.Template ),
                                OleDbHelper.MakeInParam("@Recommend",OleDbType.Integer,1,postinfo.Recommend),
								OleDbHelper.MakeInParam("@Status",OleDbType.Integer,1,postinfo.Status),
                                OleDbHelper.MakeInParam("@TopStatus",OleDbType.Integer,1,postinfo.TopStatus),
                                OleDbHelper.MakeInParam("@HomeStatus",OleDbType.Integer,1,postinfo.HomeStatus),
                                OleDbHelper.MakeInParam("@HideStatus",OleDbType.Integer,1,postinfo.HideStatus),
								OleDbHelper.MakeInParam("@PostTime",OleDbType.Date,8,postinfo.PostTime),
								OleDbHelper.MakeInParam("@UpdateTime",OleDbType.Date,8,postinfo.UpdateTime)
							};
            OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
            int newId = TypeConverter.ObjectToInt(OleDbHelper.ExecuteScalar(string.Format("select top 1 [PostId] from [{0}Posts] order by [PostId] desc",ConfigHelper.Tableprefix)));
            //if (newId > 0)
            //{
            //    OleDbHelper.ExecuteNonQuery(string.Format("update [{0}users] set [postcount]=[postcount]+1 where [userid]={0}", postinfo.UserId));
            //    OleDbHelper.ExecuteNonQuery("update [{0}sites] set [postcount]=[postcount]+1");
            //    OleDbHelper.ExecuteNonQuery(string.Format("update [{0}terms] set [count]=[count]+1 where [termid]={0}", postinfo.CategoryId));
            //}
            return newId;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="postinfo">实体</param>
        /// <returns>修改的行数</returns>
        public int UpdatePost(PostInfo postinfo)
        {
            CheckSlug(postinfo);

            string cmdText = string.Format(@"update [{0}posts] set  
                                       [CategoryId]=@CategoryId,
                                       [Title]=@Title,[Summary]=@Summary,
                                       [PostContent]=@PostContent,
                                       [Slug]=@Slug,
                                       [UserId]=@UserId,
                                       [CommentStatus]=@CommentStatus,
                                       [CommentCount]=@CommentCount,
                                       [ViewCount]=@ViewCount,
                                       [Tag]=@Tag,
                                       [UrlFormat]=@UrlFormat,
                                       [Template]=@Template,
                                       [Recommend]=@Recommend,
                                       [Status]=@Status,
                                       [TopStatus]=@TopStatus,
                                       [HomeStatus]=@HomeStatus,
                                       [HideStatus]=@HideStatus,
                                       [PostTime]=@PostTime,
                                       [UpdateTime]=@UpdateTime
                                   where [PostId]=@PostId", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                return conn.Execute(cmdText, postinfo);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns>删除的行数</returns>
        public int DeletePost(int postid)
        {
            PostInfo oldPost = GetPost(postid);        //删除前
            if (oldPost == null) throw new NotImplementedException();

            string cmdText = string.Format("delete from [{0}posts] where [PostId] = @PostId",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@PostId",OleDbType.Integer,4,postid)
							};
            int result = OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);



            //if (oldPost != null)
            //{
            //    OleDbHelper.ExecuteNonQuery(string.Format("update [{0}users] set [postcount]=[postcount]-1 where [userid]={0}", oldPost.UserId));
            //    OleDbHelper.ExecuteNonQuery("update [{0}sites] set [postcount]=[postcount]-1");
            //    OleDbHelper.ExecuteNonQuery(string.Format("update [{0}terms] set [count]=[count]-1 where [termid]={0}", oldPost.CategoryId));
            //}

            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        public PostInfo GetPost(int postid)
        {
            string cmdText = string.Format("select top 1 * from [{0}posts] where [PostId] = @PostId",ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                var list = conn.Query<PostInfo>(cmdText, new { PostId = postid });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public PostInfo GetPost(string slug)
        {
            string cmdText = string.Format("select top 1 * from [{0}posts] where [slug] = @_slug", ConfigHelper.Tableprefix);

            using (var conn = dapper.OpenConnection())
            {
                var list = conn.Query<PostInfo>(cmdText, new { _slug = slug });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }

        public List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount, int categoryId, int tagId, int userId, int recommend, int status, int topstatus, int hidestatus, string begindate, string enddate, string keyword)
        {
            string condition = " 1=1 ";

            if (categoryId != -1)
            {
                condition += " and categoryId=" + categoryId;
            }
            if (tagId != -1)
            {
                condition += " and tag like '%{" + tagId + "}%'";
            }
            if (userId != -1)
            {
                condition += " and userid=" + userId;
            }
            if (recommend != -1)
            {
                condition += " and recommend=" + recommend;
            }
            if (status != -1)
            {
                condition += " and status=" + status;
            }

            if (topstatus != -1)
            {
                condition += " and topstatus=" + topstatus;
            }

            if (hidestatus != -1)
            {
                condition += " and hidestatus=" + hidestatus;
            }

            if (!string.IsNullOrEmpty(begindate))
            {
                condition += " and PostTime>=#" + begindate + "#";
            }
            if (!string.IsNullOrEmpty(enddate))
            {
                condition += " and PostTime<#" + enddate + "#";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                condition += string.Format(" and (summary like '%{0}%' or title like '%{0}%'  )", keyword);
            }

            string cmdTotalRecord = "select count(1) from ["+ConfigHelper.Tableprefix+"posts] where " + condition;

            //   throw new Exception(cmdTotalRecord);

            recordCount = TypeConverter.ObjectToInt(OleDbHelper.ExecuteScalar(CommandType.Text, cmdTotalRecord));


            string cmdText = OleDbHelper.GetPageSql("["+ConfigHelper.Tableprefix+"Posts]", "[PostId]", "*", pageSize, pageIndex, 1, condition);



            using (var conn = dapper.OpenConnection())
            {
                var list = conn.Query<PostInfo>(cmdText, null);
                return list.ToList();
            }
        }

        public List<PostInfo> GetPostListByRelated(int postId, int rowCount)
        {
            string tags;

            PostInfo post = GetPost(postId);

            if (post != null && post.Tag.Length > 0)
            {
                tags = post.Tag;


                tags = tags.Replace("}", "},");
                string[] idList = tags.Split(',');

                string where = idList.Where(tagId => !string.IsNullOrEmpty(tagId)).Aggregate(" (", (current, tagId) => current + string.Format("  [tags] like '%{0}%' or ", tagId));
                where += " 1=2 ) and [status]=1 and [postid]<>" + postId;

                string cmdText = string.Format("select top {0} * from [{2}posts] where {1} order by [postid] desc", rowCount, where,ConfigHelper.Tableprefix);

                using (var conn = dapper.OpenConnection())
                {
                    var list = conn.Query<PostInfo>(cmdText, null);
                    return list.ToList();
                }
            }
            return new List<PostInfo>();
        }

        ///// <summary>
        ///// 根据别名获取文章ID
        ///// </summary>
        ///// <param name="slug"></param>
        ///// <returns></returns>
        //public int GetPostId(string slug)
        //{
        //    string cmdText = "select [postid] from [{0}posts] where [slug]=@slug";
        //    OleDbParameter[] prams = {  
        //                           OleDbHelper.MakeInParam("@slug",OleDbType.VarWChar,200,slug),

        //                            };
        //    return TypeConverter.ObjectToInt(OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams));

        //}

        public List<ArchiveInfo> GetArchive()
        {
            string cmdText = string.Format("select format(PostTime, 'yyyymm') as [date] ,  count(*) as [count] from [{0}posts] where [status]=1 and [hidestatus]=0  group by  format(PostTime, 'yyyymm')  order by format(PostTime, 'yyyymm') desc",ConfigHelper.Tableprefix);

            var list = new List<ArchiveInfo>();
            using (OleDbDataReader read = OleDbHelper.ExecuteReader(cmdText))
            {

                while (read.Read())
                {
                    var archive = new ArchiveInfo();
                    string date = read["date"].ToString().Substring(0, 4) + "-" + read["date"].ToString().Substring(4, 2);
                    archive.Date = Convert.ToDateTime(date);
                    // archive.Title = read["date"].ToString();
                    archive.Count = TypeConverter.ObjectToInt(read["count"]);
                    list.Add(archive);
                }
            }
            return list;

        }

        public int UpdatePostViewCount(int postId, int addCount)
        {
            string cmdText = string.Format("update [{0}posts] set [viewcount] = [viewcount] + @addcount where [postid]=@postid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@addcount",OleDbType.Integer,4,addCount),
                                OleDbHelper.MakeInParam("@postid",OleDbType.Integer,4,postId),
							};
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
        }

    }
}
