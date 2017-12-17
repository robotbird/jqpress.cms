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
        /// 添加评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        int InsertComment(CommentInfo comment);

        /// <summary>
        /// 修改评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        int UpdateComment(CommentInfo comment);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        int DeleteComment(int commentId);

        /// <summary>
        /// 获取获取评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        CommentInfo GetComment(int commentId);


        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        int DeleteCommentByPost(int postId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalRecord"></param>
        /// <param name="order"></param>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="parentId"></param>
        /// <param name="approved"></param>
        /// <param name="emailNotify"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        List<CommentInfo> GetCommentList(int pageSize, int pageIndex, out int totalRecord, int order, int userId, int postId, int parentId, int approved, int emailNotify, string keyword);

        //   List<CommentInfo> GetCommentList(int parentId);


        //  List<CommentInfo> GetCommentList(int postId,int parentId, int emailStatus);


        /// <summary>
        /// 获取某文章的评论数
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="postId">文章ID</param>
        /// <param name="incChild">是否包括子评论</param>
        /// <returns></returns>
        int GetCommentCount(int userId, int postId, bool incChild);
    }
}
