using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;

namespace Jqpress.Core.Repositories.IRepository
{
    public partial interface ICommentRepository : IRepository<CommentInfo>
    {
        /// <summary>
        /// 根据文章删除评论
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        int DeleteCommentByPost(int postId);
        /// <summary>
        /// 获取评论统计
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="incChild"></param>
        /// <returns></returns>
        int GetCommentCount(int userId, int postId, bool incChild);
        /// <summary>
        /// 获取评论分页
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
    }
}
