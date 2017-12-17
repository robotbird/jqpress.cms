using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Jqpress.Core.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Services;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;

namespace Jqpress.Core.XmlRpc
{
    /// <summary>
    /// HTTP Handler for MetaWeblog API
    /// 
    /// from BlogEngine Source
    /// </summary>
    public class MetaWeblogHelper
    {
        #region IHttpHandler Members


        /// <summary>
        /// Process the HTTP Request.  Create XMLRPC request, find method call, process it and create response object and sent it back.
        /// This is the heart of the MetaWeblog API
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string rootUrl = ConfigHelper.SiteUrl;
                XMLRPCRequest input = new XMLRPCRequest(context);
                XMLRPCResponse output = new XMLRPCResponse(input.MethodName);

                switch (input.MethodName)
                {
                    case "metaWeblog.newPost":
                        output.PostID = NewPost(input.BlogID, input.UserName, input.Password, input.Post, input.Publish);
                        break;
                    case "metaWeblog.editPost":
                        output.Completed = EditPost(input.PostID, input.UserName, input.Password, input.Post, input.Publish);
                        break;
                    case "metaWeblog.getPost":
                        output.Post = GetPost(input.PostID, input.UserName, input.Password);
                        break;
                    case "metaWeblog.newMediaObject":
                        output.MediaInfo = NewMediaObject(input.BlogID, input.UserName, input.Password, input.MediaObject, context);
                        break;
                    case "metaWeblog.getCategories":
                        output.Categories = GetCategories(input.BlogID, input.UserName, input.Password, rootUrl);
                        break;
                    case "metaWeblog.getRecentPosts":
                        output.Posts = GetRecentPosts(input.BlogID, input.UserName, input.Password, input.NumberOfPosts);
                        break;
                    case "blogger.getUsersBlogs":
                    case "metaWeblog.getUsersBlogs":
                        output.Blogs = GetUserBlogs(input.AppKey, input.UserName, input.Password, rootUrl);
                        break;
                    case "blogger.deletePost":
                        output.Completed = DeletePost(input.AppKey, input.PostID, input.UserName, input.Password, input.Publish);
                        break;
                    case "blogger.getUserInfo":
                        throw new MetaWeblogException("10", "获取用户方法未实现.");
                    case "wp.newPage":
                        throw new MetaWeblogException("10", "创建页面未实现.");
                        break;
                    case "wp.getPageList":
                    case "wp.getPages":
                        throw new MetaWeblogException("10", "获取页面列表未实现.");
                        break;
                    case "wp.getPage":
                        throw new MetaWeblogException("10", "获取页面未实现.");
                        break;
                    case "wp.editPage":
                        throw new MetaWeblogException("10", "编辑页面未实现.");
                        break;
                    case "wp.deletePage":
                        throw new MetaWeblogException("10", "删除页面未实现.");
                        break;
                    case "wp.getAuthors":
                        output.Authors = GetAuthors(input.BlogID, input.UserName, input.Password);
                        break;
                    case "wp.getTags":
                        output.Keywords = GetKeywords(input.BlogID, input.UserName, input.Password);
                        break;
                }

                output.Response(context);
            }
            catch (MetaWeblogException mex)
            {
                XMLRPCResponse output = new XMLRPCResponse("fault");
                MWAFault fault = new MWAFault();
                fault.faultCode = mex.Code;
                fault.faultString = mex.Message;
                output.Fault = fault;
                output.Response(context);
            }
            catch (Exception ex)
            {
                XMLRPCResponse output = new XMLRPCResponse("fault");
                MWAFault fault = new MWAFault();
                fault.faultCode = "0";
                fault.faultString = ex.Message;
                output.Fault = fault;
                output.Response(context);
            }
        }

        #endregion

        #region API Methods

        /// <summary>
        /// 由标签名称列表返回标签ID列表,带{},新标签自动添加
        /// 拷贝自添加文章页面
        /// </summary>
        /// <param name="tagNameList"></param>
        /// <returns></returns>
        private string GetTagIdList(List<string> names)
        {
            //if (string.IsNullOrEmpty(tagNames))
            //{
            //    return string.Empty;
            //}
            string tagIds = string.Empty;
            //tagNames = tagNames.Replace("，", ",");

            //string[] names = tagNames.Split(',');
            TagService tagService = new TagService();
            foreach (string n in names)
            {
                if (!string.IsNullOrEmpty(n))
                {
                    TagInfo t = new TagService().GetTag(Jqpress.Framework.Web.HttpHelper.HtmlEncode(n));

                    if (t == null)
                    {
                        t = new TagInfo();

                        t.PostCount = 0;
                        t.CreateTime = DateTime.Now;
                        t.Description = Jqpress.Framework.Web.HttpHelper.HtmlEncode(n);
                        t.SortNum = 1000;
                        t.CateName = Jqpress.Framework.Web.HttpHelper.HtmlEncode(n);
                        t.PageName = Jqpress.Framework.Utils.StringHelper.FilterPageName(n, "tag", false);

                        t.TagId = new TagService().InsertTag(t);
                    }
                    tagIds += "{" + t.TagId + "}";
                }
            }
            return tagIds;
        }


        /// <summary>
        /// 添加或修改文章
        /// </summary>
        /// <param name="blogID"></param>
        /// <param name="postID"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sentPost"></param>
        /// <param name="publish"></param>
        /// <param name="operate"></param>
        /// <returns></returns>
        private int NewOrUpdatePost(string blogID, string postID, string userName, string password, MWAPost sentPost, bool publish, OperateType operate)
        {
            ValidateRequest(userName, password);

            PostInfo post = new PostInfo();
            PostService _postService = new PostService();

            if (operate == OperateType.Update)
            {
                post = new PostService().GetPost(Jqpress.Framework.Utils.TypeConverter.StrToInt(postID, 0));

            }
            else
            {
                post.CommentCount = 0;
                post.ViewCount = 0;
                post.PostTime = DateTime.Now;

                UserInfo user = (new UserService()).GetUser(userName);
                if (user != null)
                {
                    post.UserId = user.UserId;
                }
            }

            post.Title = Jqpress.Framework.Web.HttpHelper.HtmlEncode(sentPost.title);
            post.PostContent = sentPost.description;
            post.Status = publish == true ? 1 : 0;
            post.PageName = Jqpress.Framework.Utils.StringHelper.FilterPageName(sentPost.pagename, "post", true);
            post.Summary = sentPost.excerpt;

            post.UrlFormat = (int)PostUrlFormat.Default;
            post.Template = string.Empty;
            post.Recommend = 0;
            post.TopStatus = 0;
            post.PostStatus = 0;
            post.UpdateTime = DateTime.Now;

            if (sentPost.commentPolicy != "")
            {
                if (sentPost.commentPolicy == "1")
                    post.CommentStatus = 1;
                else
                    post.CommentStatus = 0;
            }


            foreach (string item in sentPost.categories)
            {
                CategoryInfo cat;
                if (LookupCategoryGuidByName(item, out cat))
                {
                    post.CategoryId = cat.CategoryId;
                }
                else
                {
                    CategoryInfo newcat = new CategoryInfo();
                    newcat.PostCount = 0;
                    newcat.CreateTime = DateTime.Now;
                    newcat.Description = "由离线工具创建";
                    newcat.SortNum = 1000;
                    newcat.CateName = Jqpress.Framework.Web.HttpHelper.HtmlEncode(item);
                    newcat.PageName = Jqpress.Framework.Utils.StringHelper.FilterPageName(item, "cate", false);

                    newcat.CategoryId = new CategoryService().InsertCategory(newcat);
                    post.CategoryId = newcat.CategoryId;
                }
            }
            post.Tag = GetTagIdList(sentPost.tags);

            if (operate == OperateType.Update)
            {
                new PostService().UpdatePost(post);
            }
            else
            {
                post.PostId = new PostService().InsertPost(post);

                //    SendEmail(p);
            }

            return post.PostId;
        }

        /// <summary>
        /// metaWeblog.newPost
        /// </summary>
        /// <param name="blogID">always 1000 in BlogEngine since it is a singlar blog instance</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="sentPost">struct with post details</param>
        /// <param name="publish">mark as published?</param>
        /// <returns>postID as string</returns>
        internal string NewPost(string blogID, string userName, string password, MWAPost sentPost, bool publish)
        {
            ValidateRequest(userName, password);

            return NewOrUpdatePost(blogID, "", userName, password, sentPost, publish, OperateType.Insert).ToString();
        }

        /// <summary>
        /// metaWeblog.editPost
        /// </summary>
        /// <param name="postID">post guid in string format</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="sentPost">struct with post details</param>
        /// <param name="publish">mark as published?</param>
        /// <returns>1 if successful</returns>
        internal bool EditPost(string postID, string userName, string password, MWAPost sentPost, bool publish)
        {
            ValidateRequest(userName, password);

            NewOrUpdatePost("", postID, userName, password, sentPost, publish, OperateType.Update);

            return true;
        }

        /// <summary>
        /// metaWeblog.getPost
        /// </summary>
        /// <param name="postID">post guid in string format</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <returns>struct with post details</returns>
        internal MWAPost GetPost(string postID, string userName, string password)
        {
            ValidateRequest(userName, password);

            MWAPost sendPost = new MWAPost();
            PostService _postService = new PostService();

            PostInfo post = new PostService().GetPost(Jqpress.Framework.Utils.TypeConverter.StrToInt(postID, 0));

            sendPost.postID = post.PostId.ToString();
            sendPost.postDate = post.PostTime;
            sendPost.title = Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.Title);
            sendPost.description = post.PostContent;
            sendPost.link = post.Url;
            sendPost.pagename = Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.PageName);
            sendPost.excerpt = post.Summary;
            if (post.CommentStatus == 1)
            {
                sendPost.commentPolicy = "1";
            }
            else
            {
                sendPost.commentPolicy = "0";
            }

            sendPost.publish = post.Status == 1 ? true : false;

            List<string> cats = new List<string>();
            cats.Add(Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.Category.CateName));
            sendPost.categories = cats;

            List<string> tags = new List<string>();
            for (int i = 0; i < post.Tags.Count; i++)
            {
                tags.Add(Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.Tags[i].CateName));
            }
            sendPost.tags = tags;

            return sendPost;
        }

        /// <summary>
        /// metaWeblog.newMediaObject
        /// </summary>
        /// <param name="blogID">always 1000 in BlogEngine since it is a singlar blog instance</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="mediaObject">struct with media details</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>struct with url to media</returns>
        internal MWAMediaInfo NewMediaObject(string blogID, string userName, string password, MWAMediaObject mediaObject, HttpContext request)
        {
            ValidateRequest(userName, password);

            MWAMediaInfo mediaInfo = new MWAMediaInfo();

            string newPath = "upfiles/" + DateTime.Now.ToString("yyyyMM") + "/";
            string saveFolder = HttpContext.Current.Server.MapPath(ConfigHelper.SitePath + newPath);
            string fileName = mediaObject.name;

            // Check/Create Folders & Fix fileName
            if (mediaObject.name.LastIndexOf('/') > -1)
            {

                fileName = mediaObject.name.Substring(mediaObject.name.LastIndexOf('/') + 1);
            }

            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            if (File.Exists(saveFolder + fileName))
            {
                // Find unique fileName
                for (int count = 1; count < 30000; count++)
                {
                    string tempFileName = fileName.Insert(fileName.LastIndexOf('.'), "_" + count);
                    if (!File.Exists(saveFolder + tempFileName))
                    {
                        fileName = tempFileName;
                        break;
                    }
                }
            }


            // Save File
            FileStream fs = new FileStream(saveFolder + fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(mediaObject.bits);
            bw.Close();

            mediaInfo.url = ConfigHelper.SiteUrl + newPath + fileName;
            return mediaInfo;
        }

        /// <summary>
        /// metaWeblog.getCategories
        /// </summary>
        /// <param name="blogID">always 1000 in BlogEngine since it is a singlar blog instance</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="rootUrl">The root URL.</param>
        /// <returns>array of category structs</returns>
        internal List<MWACategory> GetCategories(string blogID, string userName, string password, string rootUrl)
        {
            ValidateRequest(userName, password);

            List<MWACategory> categories = new List<MWACategory>();

            foreach (CategoryInfo cat in new CategoryService().GetCategoryList())
            {
                MWACategory temp = new MWACategory();
                temp.title = Jqpress.Framework.Web.HttpHelper.HtmlDecode(cat.CateName);
                temp.description = Jqpress.Framework.Web.HttpHelper.HtmlDecode(cat.Description);
                temp.htmlUrl = cat.Url;
                temp.rssUrl = cat.FeedUrl;
                categories.Add(temp);
            }

            return categories;
        }

        /// <summary>
        /// wp.getTags
        /// </summary>
        /// <param name="blogID"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>list of tags</returns>
        internal List<string> GetKeywords(string blogID, string userName, string password)
        {
            ValidateRequest(userName, password);

            List<string> keywords = new List<string>();

            foreach (TagInfo tag in (new TagService()).GetTagList(100))
            {
                keywords.Add(Jqpress.Framework.Web.HttpHelper.HtmlDecode(tag.CateName));
            }

            return keywords;
        }

        /// <summary>
        /// metaWeblog.getRecentPosts
        /// </summary>
        /// <param name="blogID">always 1000 in BlogEngine since it is a singlar blog instance</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="numberOfPosts">number of posts to return</param>
        /// <returns>array of post structs</returns>
        internal List<MWAPost> GetRecentPosts(string blogID, string userName, string password, int numberOfPosts)
        {
            ValidateRequest(userName, password);

            List<MWAPost> sendPosts = new List<MWAPost>();

            int userid = 0;
            UserInfo user = (new UserService()).GetUser(userName);
            if (user != null)
            {
                userid = user.UserId;
            }
            PostService _postService = new PostService();

            List<PostInfo> posts = new PostService().GetPostList(numberOfPosts, "", userid, -1, -1, -1, -1,-1);

            foreach (PostInfo post in posts)
            {
                MWAPost tempPost = new MWAPost();
                List<string> tempCats = new List<string>();
                List<string> tempTags = new List<string>();

                tempPost.postID = post.PostId.ToString();
                tempPost.postDate = post.PostTime;
                tempPost.title = Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.Title);
                tempPost.description = post.PostContent;
                tempPost.link = post.Url;
                tempPost.pagename = Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.PageName);
                tempPost.excerpt = post.Summary;
                if (post.CommentStatus == 1)
                    tempPost.commentPolicy = "";
                else
                    tempPost.commentPolicy = "0";
                tempPost.publish = post.Status == 1 ? true : false;

                tempCats.Add(Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.Category.CateName));
                tempPost.categories = tempCats;

                for (int i = 0; i < post.Tags.Count; i++)
                {
                    tempTags.Add(Jqpress.Framework.Web.HttpHelper.HtmlDecode(post.Tags[i].CateName));
                }
                tempPost.tags = tempTags;

                sendPosts.Add(tempPost);
            }
            return sendPosts;
        }

        /// <summary>
        /// blogger.getUsersBlogs
        /// </summary>
        /// <param name="appKey">Key from application.  Outdated methodology that has no use here.</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="rootUrl">The root URL.</param>
        /// <returns>array of blog structs</returns>
        internal List<MWABlogInfo> GetUserBlogs(string appKey, string userName, string password, string rootUrl)
        {
            ValidateRequest(userName, password);

            List<MWABlogInfo> blogs = new List<MWABlogInfo>();

            MWABlogInfo temp = new MWABlogInfo();
            temp.url = rootUrl;
            temp.blogID = "1000";
            temp.blogName = SiteConfig.GetSetting().SiteName;
            blogs.Add(temp);

            return blogs;
        }

        /// <summary>
        /// blogger.deletePost
        /// </summary>
        /// <param name="appKey">Key from application.  Outdated methodology that has no use here.</param>
        /// <param name="postID">post guid in string format</param>
        /// <param name="userName">login username</param>
        /// <param name="password">login password</param>
        /// <param name="publish">mark as published?</param>
        /// <returns></returns>
        internal bool DeletePost(string appKey, string postID, string userName, string password, bool publish)
        {
            ValidateRequest(userName, password);
            var _postService = new PostService();
            new PostService().DeletePost(Jqpress.Framework.Utils.TypeConverter.StrToInt(postID, 0));
            return true;
        }

        internal List<MWAAuthor> GetAuthors(string blogID, string userName, string password)
        {
            ValidateRequest(userName, password);

            List<MWAAuthor> authors = new List<MWAAuthor>();

            MWAAuthor temp = new MWAAuthor();
            UserInfo user = (new UserService()).GetUser(userName);
            if (user != null)
            {
                temp.user_id = user.UserId.ToString();
                temp.user_login = user.UserName;
                temp.display_name = user.NickName;
                temp.user_email = user.Email;
                temp.meta_value = user.Description;
                authors.Add(temp);
            }
            return authors;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 验证用户
        /// Checks username and password.  Throws error if validation fails.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        private void ValidateRequest(string userName, string password)
        {
            password = Jqpress.Framework.Utils.EncryptHelper.MD5(password);
            if ((new UserService()).GetUser(userName, password) == null)
            {
                throw new MetaWeblogException("11", "用户名或密码错误");
            }
        }

        /// <summary>
        /// 查找分类，存在返回TRUE和分类,不存在返回FALSE
        /// Returns Category Guid from Category name.
        /// </summary>
        /// <remarks>
        /// Reverse dictionary lookups are ugly.
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        private bool LookupCategoryGuidByName(string name, out  CategoryInfo cat)
        {
            name = Jqpress.Framework.Web.HttpHelper.HtmlEncode(name);

            cat = new CategoryInfo();
            foreach (CategoryInfo item in new CategoryService().GetCategoryList())
            {
                if (item.CateName == name)
                {
                    cat = item;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Exception specifically for MetaWeblog API.  Error (or fault) responses 
    /// request a code value.  This is our chance to add one to the exceptions
    /// which can be used to produce a proper fault.
    /// </summary>
    [Serializable()]
    public class MetaWeblogException : Exception
    {
        /// <summary>
        /// Constructor to load properties
        /// </summary>
        /// <param name="code">Fault code to be returned in Fault Response</param>
        /// <param name="message">Message to be returned in Fault Response</param>
        public MetaWeblogException(string code, string message)
            : base(message)
        {
            _code = code;
        }

        private string _code;
        /// <summary>
        /// Code is actually for Fault Code.  It will be passed back in the 
        /// response along with the error message.
        /// </summary>
        public string Code
        {
            get { return _code; }
        }
    }


    # region struct

    /// <summary>
    /// MetaWeblog Category struct
    /// returned as an array from GetCategories
    /// </summary>
    internal struct MWACategory
    {
        /// <summary>
        /// Category title
        /// </summary>
        public string description;
        /// <summary>
        /// Url to thml display of category
        /// </summary>
        public string htmlUrl;
        /// <summary>
        /// Url to RSS for category
        /// </summary>
        public string rssUrl;
        /// <summary>
        /// The guid of the category
        /// </summary>
        public string id;
        /// <summary>
        /// The title/name of the category
        /// </summary>
        public string title;
    }

    /// <summary>
    /// MetaWeblog BlogInfo struct
    /// returned as an array from getUserBlogs
    /// </summary>
    internal struct MWABlogInfo
    {
        /// <summary>
        /// Blog Url
        /// </summary>
        public string url;
        /// <summary>
        /// Blog ID (Since BlogEngine.NET is single instance this number is always 10.
        /// </summary>
        public string blogID;
        /// <summary>
        /// Blog Title
        /// </summary>
        public string blogName;
    }

    /// <summary>
    /// MetaWeblog Fault struct
    /// returned when error occurs
    /// </summary>
    internal struct MWAFault
    {
        /// <summary>
        /// Error code of Fault Response
        /// </summary>
        public string faultCode;
        /// <summary>
        /// Message of Fault Response
        /// </summary>
        public string faultString;
    }

    /// <summary>
    /// MetaWeblog MediaObject struct
    /// passed in the newMediaObject call
    /// </summary>
    internal struct MWAMediaObject
    {
        /// <summary>
        /// Name of media object (filename)
        /// </summary>
        public string name;
        /// <summary>
        /// Type of file
        /// </summary>
        public string type;
        /// <summary>
        /// Media
        /// </summary>
        public byte[] bits;
    }

    /// <summary>
    /// MetaWeblog MediaInfo struct
    /// returned from NewMediaObject call
    /// </summary>
    internal struct MWAMediaInfo
    {
        /// <summary>
        /// Url that points to Saved MediaObejct
        /// </summary>
        public string url;
    }

    /// <summary>
    /// MetaWeblog Post struct
    /// used in newPost, editPost, getPost, recentPosts
    /// not all properties are used everytime.
    /// </summary>
    internal struct MWAPost
    {
        /// <summary>
        /// PostID Guid in string format
        /// </summary>
        public string postID;
        /// <summary>
        /// Title of Blog Post
        /// </summary>
        public string title;
        /// <summary>
        /// Link to Blog Post
        /// </summary>
        public string link;
        /// <summary>
        /// Content of Blog Post
        /// </summary>
        public string description;
        /// <summary>
        /// List of Categories assigned for Blog Post
        /// </summary>
        public List<string> categories;
        /// <summary>
        /// List of Tags assinged for Blog Post
        /// </summary>
        public List<string> tags;
        /// <summary>
        /// Display date of Blog Post (DateCreated)
        /// </summary>
        public DateTime postDate;
        /// <summary>
        /// Whether the Post is published or not.
        /// </summary>
        public bool publish;
        /// <summary>
        /// PageName of post
        /// </summary>
        public string pagename;
        /// <summary>
        /// CommentPolicy (Allow/Deny)
        /// </summary>
        public string commentPolicy;
        /// <summary>
        /// Excerpt
        /// </summary>
        public string excerpt;
        /// <summary>
        /// wp_author_id
        /// </summary>
        public string author;

    }

    /// <summary>
    /// wp Page Struct
    /// </summary>
    internal struct MWAPage
    {
        /// <summary>
        /// PostID Guid in string format
        /// </summary>
        public string pageID;
        /// <summary>
        /// Title of Blog Post
        /// </summary>
        public string title;
        /// <summary>
        /// Link to Blog Post
        /// </summary>
        public string link;
        /// <summary>
        /// Content of Blog Post
        /// </summary>
        public string description;
        /// <summary>
        /// Display date of Blog Post (DateCreated)
        /// </summary>
        public DateTime pageDate;
        /// <summary>
        /// Convert Breaks
        /// </summary>
        public string mt_convert_breaks;
        /// <summary>
        /// Page Parent ID
        /// </summary>
        public string pageParentID;
        /// <summary>
        /// Page keywords
        /// </summary>
        public string mt_keywords;
    }

    /// <summary>
    /// wp Author struct
    /// </summary>
    internal struct MWAAuthor
    {
        /// <summary>
        /// userID - Specs call for a int, but our ID is a string.
        /// </summary>
        public string user_id;
        /// <summary>
        /// user login name
        /// </summary>
        public string user_login;
        /// <summary>
        /// display name
        /// </summary>
        public string display_name;
        /// <summary>
        /// user email
        /// </summary>
        public string user_email;
        /// <summary>
        /// nothing to see here.
        /// </summary>
        public string meta_value;
    }

    #endregion
}
