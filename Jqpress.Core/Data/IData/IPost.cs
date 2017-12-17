using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;

namespace Jqpress.Blog.Data.IData
{
   public partial interface IDataProvider
    {
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        List<PostInfo> GetPostList();


        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="_postinfo"></param>
        /// <returns></returns>
        int InsertPost(PostInfo _postinfo);

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="_postinfo"></param>
        /// <returns></returns>
        int UpdatePost(PostInfo _postinfo);

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="postid"></param>
        /// <returns></returns>
        int DeletePost(int postid);

        /// <summary>
        /// 获取文章
        /// </summary>
        /// <param name="postid"></param>
        /// <returns></returns>
        PostInfo GetPost(int postid);

        /// <summary>
        /// 获取文章
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        PostInfo GetPost(string slug);



        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="type"></param>
        /// <param name="categoryId"></param>
        /// <param name="tagId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="topstatus"></param>
        /// <param name="begindate"></param>
        /// <param name="enddate"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount, int categoryId, int tagId, int userId, int recommend, int status, int topstatus, int PostStatus, string begindate, string enddate, string keyword);

        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        List<PostInfo> GetPostListByRelated(int postId, int rowCount);

        /// <summary>
        /// 获取归档
        /// </summary>
        /// <returns></returns>
        List<ArchiveInfo> GetArchive();


        /// <summary>
        /// 更新文章浏览量
        /// </summary>
        /// <param name="postId">文章Id</param>
        /// <param name="addCount">增量</param>
        /// <returns></returns>
        int UpdatePostViewCount(int postId, int addCount);
    }
}
