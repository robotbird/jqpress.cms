using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Repositories.Repository;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Services
{
    public class CommentService
    {
        private ICommentRepository _commentRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public CommentService()
            : this(new CommentRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="commentRepository"></param>
        public CommentService(ICommentRepository commentRepository)
        {
            this._commentRepository = commentRepository;
        }
        #endregion
        /// <summary>
        /// 最近评论列表
        /// </summary>
        private  List<CommentInfo> _recentcomments;


        /// <summary>
        /// lock
        /// </summary>
        private  object lockHelper = new object();

        /// <summary>
        /// 加载最近评论
        /// </summary>
        private  void LoadRecentComment(int rowCount)
        {
            lock (lockHelper)
            {
                _recentcomments = GetCommentList(rowCount, 1, -1, -1, 0, (int)ApprovedStatus.Success, -1, string.Empty);
            }
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public  int InsertComment(CommentInfo comment)
        {
            int result = _commentRepository.Insert(comment);

            //统计
            new StatisticsService().UpdateStatisticsCommentCount(1);

            //用户
            new UserService().UpdateUserCommentCount(comment.UserId, 1);

            //文章
            //TODO:postservice
            //PostService.UpdatePostCommentCount(comment.PostId, 1);


            _recentcomments = null;

            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public  int UpdateComment(CommentInfo comment)
        {
            _recentcomments = null;

            return _commentRepository.Update(comment);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public  int DeleteComment(int commentId)
        {
            CommentInfo comment = GetComment(commentId);

            int result = _commentRepository.Delete(new CommentInfo { CommentId= commentId});

            //统计
            new StatisticsService().UpdateStatisticsCommentCount(-1);

            if (comment != null)
            {
                //用户
                new UserService().UpdateUserCommentCount(comment.UserId, -1);
                //文章
                //TODO:postservice
                //new PostService().UpdatePostCommentCount(comment.PostId, -1);
            }

            _recentcomments = null;

            return result;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public  CommentInfo GetComment(int commentId)
        {
            CommentInfo comment =_commentRepository.GetById(commentId);
            return comment;
        }


        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="parentId"></param>
        /// <param name="approved"></param>
        /// <param name="emailNotify"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public  List<CommentInfo> GetCommentList(int rowCount, int order, int userId, int postId, int parentId, int approved, int emailNotify, string keyword)
        {
            int totalRecord = 0;
            return GetCommentList(rowCount, 1, out totalRecord, order, userId, postId, parentId, approved, emailNotify, keyword);
        }

        /// <summary>
        /// 最近评论
        /// </summary>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public  List<CommentInfo> GetCommentListByRecent(int rowCount)
        {
            if (_recentcomments == null)
            {
                LoadRecentComment(rowCount);
            }
            return _recentcomments;

            //string cacheKey = "/comment/recent";// +rowCount;

            //List<CommentInfo> list = (List<CommentInfo>)Caches.Get(cacheKey);
            //if (list == null)
            //{
            //    int totalRecord = 0;
            //    list = GetCommentList(rowCount, 1, out totalRecord, 1, -1, -1, 0, (int)ApprovedStatus.Success, -1, string.Empty);

            //    Caches.Add(cacheKey, list);
            //}
            //return list;
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalRecord"></param>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="parentId"></param>
        /// <param name="approved"></param>
        /// <param name="emailStatus"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public  List<CommentInfo> GetCommentList(int pageSize, int pageIndex, out int totalRecord, int order, int userId, int postId, int parentId, int approved, int emailNotify, string keyword)
        {
            List<CommentInfo> list = _commentRepository.GetCommentList(pageSize, pageIndex, out totalRecord, order, userId, postId, parentId, approved, emailNotify, keyword);

            int floor = 1;
            foreach (CommentInfo comment in list)
            {

                comment.Floor = pageSize * (pageIndex - 1) + floor;
                floor++;
            }
            return list;
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalRecord"></param>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="parentId"></param>
        /// <param name="approved"></param>
        /// <param name="emailStatus"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public  IPagedList<CommentInfo> GetCommentListPage(int pageSize, int pageIndex, out int recordCount, int order, int userId, int postId, int parentId, int approved, int emailNotify, string keyword)
        {
            List<CommentInfo> list = _commentRepository.GetCommentList(pageSize, pageIndex, out recordCount, order, userId, postId, parentId, approved, emailNotify, keyword);

            int floor = 1;
            foreach (CommentInfo comment in list)
            {

                comment.Floor = pageSize * (pageIndex - 1) + floor;
                floor++;
            }
            return new PagedList<CommentInfo>(list, pageIndex - 1, pageSize, recordCount);
        }

        /// <summary>
        /// 根据日志ID删除评论
        /// </summary>
        /// <param name="postId">日志ID</param>
        /// <returns></returns>
        public  int DeleteCommentByPost(int postId)
        {
            int result =_commentRepository.DeleteCommentByPost(postId);

            new StatisticsService().UpdateStatisticsCommentCount(-result);

            _recentcomments = null;

            return result;
        }

        /// <summary>
        /// 统计评论数
        /// </summary>
        /// <param name="incChild"></param>
        /// <returns></returns>
        public  int GetCommentCount(bool incChild)
        {
            return GetCommentCount(-1, incChild);
        }

        /// <summary>
        /// 统计评论数
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public  int GetCommentCount(int postId, bool incChild)
        {
            return GetCommentCount(-1, postId, incChild);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="incChild"></param>
        /// <returns></returns>
        public  int GetCommentCount(int userId, int postId, bool incChild)
        {

            return _commentRepository.GetCommentCount(userId, postId, incChild);
        }
    }
}
