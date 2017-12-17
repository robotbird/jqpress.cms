using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Jqpress.Core.Services;
using Jqpress.Framework.Utils;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 评论实体
    /// </summary>
    public class CommentInfo
    {

        #region 非字段
        private string _title;
        private string _url;
        private string _link;
        private string _authorlink;

        private int _floor;

        private string _gravatarcode;



        /// <summary>
        /// 标题(实为过滤html的正文某长度文字)
        /// </summary>
        public string Title
        {
            //set { _title = value; }
            get { return StringHelper.CutString(StringHelper.RemoveHtml(this.Contents), 0, 20); }
        }


        /// <summary>
        /// 地址(还没考虑分页)
        /// </summary>
        public string Url
        {
            // set { _url = value; }
            get
            {
                PostInfo post = (new PostService()).GetPost(this.PostId);
                if (post != null)
                {
                    return string.Format("{0}#comment-{1}", post.Url, this.CommentId);
                }
                return "###";
            }
        }

        /// <summary>
        /// 评论连接
        /// </summary>
        public string Link
        {
            //  set { _link = value; }
            get
            {
                return string.Format("<a href=\"{0}\" title=\"{1} 发表于 {2}\">{3}</a>", this.Url, this.Author, this.CreateTime, this.Title);
            }
        }



        /// <summary>
        /// 作者连接
        /// </summary>
        public string AuthorLink
        {
            //   set { _authorlink = value; }
            get
            {
                if (this.UserId > 0)
                {
                    UserInfo user = (new UserService()).GetUser(this.UserId);
                    if (user != null)
                    {
                        return user.Link;
                    }

                }
                else if (Jqpress.Framework.Utils.Validate.IsHttpUrl(this.AuthorUrl))
                {
                    return string.Format("<a href=\"{0}\" target=\"_blank\" title=\"{1}\">{1}</a>", this.AuthorUrl, this.Author);
                }
                return this.Author;
            }
        }

        /// <summary>
        /// 层次
        /// </summary>
        public int Floor
        {
            set { _floor = value; }
            get { return _floor; }
        }

        /// <summary>
        /// Gravatar 加密后的字符串
        /// </summary>
        public string GravatarCode
        {
            //  set { _gravatarcode = value; }
            get
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(this.Email, "MD5").ToLower();
            }
        }

        /// <summary>
        /// 评论对应文章
        /// </summary>
        public PostInfo Post
        {
            get
            {
                PostInfo post = (new PostService()).GetPost(this.PostId);
                if (post != null)
                {
                    return post;
                }
                return new PostInfo();
            }
        }

        #endregion

        /// <summary>
        /// 评论ID
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 文章ID
        /// </summary>
        public int PostId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 网址
        /// </summary>
        public string AuthorUrl { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// 邮件通知  1:通知,0:不通知
        /// </summary>
        public int EmailNotify { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 审核
        /// </summary>
        public int Approved { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
