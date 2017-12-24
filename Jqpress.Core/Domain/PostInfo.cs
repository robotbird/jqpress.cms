﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Configuration;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Services;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;
using Jqpress.Framework.DbProvider;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 文章实体
    /// 是否要加个排序字段
    /// 是否要加个图片URL字段
    /// </summary>
    [Table("jq_post")]
    public class PostInfo : IComparable<PostInfo>
    {
        public int CompareTo(PostInfo other)
        {

            return other.PostId.CompareTo(this.PostId);
        }

        private int _urltype;
        private int _commentstatus;
        private string _template = "post.cshtml";
        #region 非字段

        private string _url;
        private string _link;
        private string _detail;
        private UserInfo _user;
        private CategoryInfo _category;
        private List<TagInfo> _tags;
        private List<PostInfo> _relatedposts;

        /// <summary>
        /// 内容地址
        /// </summary>
        public string Url
        {
            get
            {
                string url = string.Empty;
                switch ((PostUrlFormat)this.UrlFormat)
                {

                    case PostUrlFormat.PageName:
                        // url = string.Format("{0}post/{1}", ConfigHelper.SiteUrl, !string.IsNullOrEmpty(this.PageName) ? Jqpress.Framework.Web.HttpHelper.UrlEncode(this.PageName) : this.PostId.ToString());
                        url = string.Format("{0}post/{1}", ConfigHelper.SiteUrl, this.PostId.ToString());
                        break;

                    case PostUrlFormat.Date:
                    default:
                        url = string.Format("{0}post/{1}/{2}", ConfigHelper.SiteUrl, this.PostTime.ToString(@"yyyy\/MM\/dd"), !string.IsNullOrEmpty(this.PageName) ? Jqpress.Framework.Web.HttpHelper.UrlEncode(this.PageName) : this.PostId.ToString());
                        break;
                }
                return url;
            }
        }

        /// <summary>
        /// 分页URL
        /// </summary>
        public string PageUrl
        {
            get
            {
                string url = string.Empty;
                switch ((PostUrlFormat)this.UrlFormat)
                {

                    case PostUrlFormat.PageName:

                        url = string.Format("{0}post/{1}/page/{2}{3}", ConfigHelper.SiteUrl, !string.IsNullOrEmpty(this.PageName) ? Jqpress.Framework.Web.HttpHelper.UrlEncode(this.PageName) : this.PostId.ToString(), "{0}", SiteConfig.GetSetting().RewriteExtension);
                        break;

                    case PostUrlFormat.Date:
                    default:
                        url = string.Format("{0}post/{1}/{2}/page/{3}{4}", ConfigHelper.SiteUrl, this.PostTime.ToString(@"yyyy\/MM\/dd"), !string.IsNullOrEmpty(this.PageName) ? Jqpress.Framework.Web.HttpHelper.UrlEncode(this.PageName) : this.PostId.ToString(), "{0}", SiteConfig.GetSetting().RewriteExtension);
                        break;
                }
                return url;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        public string Link
        {
            get { return string.Format("<a href=\"{0}\">{1}</a>", this.Url, this.Title); }
        }

        /// <summary>
        /// 订阅URL
        /// </summary>
        public string FeedUrl
        {
            get
            {
                return string.Format("{0}feed/comment/postid/{1}{2}", ConfigHelper.SiteUrl, this.PostId);
            }
        }

        /// <summary>
        /// 订阅连接
        /// </summary>
        public string FeedLink
        {
            get
            {
                return string.Format("<a href=\"{0}\" title=\"订阅:{1} 的评论\">订阅</a>", this.FeedUrl, this.Title);
            }
        }

        /// <summary>
        /// 文章 详情
        /// 根据设置而定,可为摘要,正文
        /// </summary>
        public string Detail
        {
            get
            {
                switch (SiteConfig.GetSetting().PostShowType)
                {
                    case 1:
                        return string.Empty;
                    case 2:
                    default:
                        return this.Summary;

                    case 3:
                        return StringHelper.CutString(StringHelper.RemoveHtml(this.PostContent), 0, 200);
                    case 4:
                        return this.PostContent;
                }
            }
        }

        /// <summary>
        /// Rss 详情
        /// 根据设置而定,可为摘要,正文,前200字,空等
        /// </summary>
        public string FeedDetail
        {
            get
            {
                switch (SiteConfig.GetSetting().RssShowType)
                {
                    case 1:
                        return string.Empty;
                    case 2:
                    default:
                        return this.Summary;

                    case 3:
                        return StringHelper.CutString(StringHelper.RemoveHtml(this.PostContent), 0, 200);
                    case 4:
                        return this.PostContent;
                }
            }
        }

        /// <summary>
        /// 作者
        /// </summary>
        public UserInfo Author
        {
            get
            {
                UserInfo user = (new UserService()).GetUser(this.UserId);
                if (user != null)
                {
                    return user;
                }
                return new UserInfo();

            }
        }

        /// <summary>
        /// 所属分类
        /// </summary>
        public CategoryInfo Category
        {
            get
            {
                CategoryInfo category = new Jqpress.Core.Services.CategoryService().GetCategory(this.CategoryId);
                if (category != null)
                {
                    return category;
                }
                return new CategoryInfo();
            }
        }

        /// <summary>
        /// 对应标签
        /// </summary>
        public List<TagInfo> Tags
        {
            get
            {
                string temptags = this.Tag.Replace("{", "").Replace("}", ",");

                if (temptags.Length > 0)
                {
                    temptags = temptags.TrimEnd(',');
                }
                return (new TagService()).GetTagList(temptags);
            }
        }

        /// <summary>
        /// 下一篇文章
        /// </summary>
        public PostInfo Next
        {
            get
            {
                List<PostInfo> list = (new PostService()).GetPostList();
                PostInfo post = list.Find(p => p.PostStatus == 0 && p.Status == 1 && p.PostId > this.PostId);
                return post != null ? post : new PostInfo();
            }
        }

        /// <summary>
        /// 上一篇文章
        /// </summary>
        public PostInfo Previous
        {
            get
            {

                List<PostInfo> list = (new PostService()).GetPostList();

                PostInfo post = list.Find(p => p.PostStatus == 0 && p.Status == 1 && p.PostId < this.PostId);

                return post != null ? post : new PostInfo();
            }
        }

        /// <summary>
        /// 相关文章
        /// </summary>
        public List<PostInfo> RelatedPosts
        {
            get
            {
                if (string.IsNullOrEmpty(this.Tag))
                {
                    return new List<PostInfo>();
                }

                List<PostInfo> list = (new PostService()).GetPostList().FindAll(p => p.PostStatus == 0 && p.Status == 1);
                string tags = this.Tag.Replace("}", "},");
                tags = tags.TrimEnd(',');

                string[] temparray = tags.Split(',');

                int num = 0;
                List<PostInfo> list2 = list.FindAll(p =>
                {
                    if (num >= SiteConfig.GetSetting().PostRelatedCount)
                    {
                        return false;
                    }

                    foreach (string tag in temparray)
                    {
                        if (p.Tag.IndexOf(tag) != -1 && p.PostId != this.PostId)
                        {
                            num++;
                            return true;
                        }
                    }
                    return false;

                });


                return list2;
            }
        }

        /// <summary>
        /// draft
        /// </summary>
        public string StatusStr
        {
            get
            {
                return Status == 0 ? "[草稿]" : "";
            }
        }

        /// <summary>
        /// description of PostStatus
        /// </summary>
        public string HideStr
        {
            get
            {
                return PostStatus == 1 ? "[私密]" : "";
            }
        }

        #endregion

        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int PostId { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string TitlePic { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string PostContent { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public string Template
        {
            get { return _template; }
            set { _template = value; }
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 是否允许评论
        /// </summary>
        public int CommentStatus
        {
            set { _commentstatus = value; }
            get
            {
                if (_commentstatus == 1 && SiteConfig.GetSetting().CommentStatus == 1)
                {
                    return 1;
                }
                return 0;

            }
        }

        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 点击数
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// URL类型,见枚举PostUrlFormat
        /// </summary>
        public int UrlFormat
        {
            set { _urltype = value; }
            get { return Jqpress.Core.Configuration.SiteConfig.GetSetting().UrlFormatType; }
        }

        /// <summary>
        /// 推荐
        /// </summary>
        public int Recommend { get; set; }

        /// <summary>
        /// 置顶
        /// </summary>
        public int TopStatus { get; set; }
        /// <summary>
        /// 是否首页显示
        /// </summary>
        public int HomeStatus { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 隐藏于列表
        /// </summary>
        public int PostStatus { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime PostTime { get; set; }
        /// <summary>
        /// 时间
        /// </summary>更新
        public DateTime UpdateTime { get; set; }
    }
}
