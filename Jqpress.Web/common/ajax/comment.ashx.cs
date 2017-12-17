using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using Jqpress.Framework.Utils;
using Jqpress.Blog.Entity;
using Jqpress.Blog.Services;
using Jqpress.Blog.Common;
using Jqpress.Blog.Configuration;
using Jqpress.Blog.Entity.Enum;
using Jqpress.Framework.Configuration;


namespace Jqpress.Web.common
{
    /// <summary>
    /// $codebehindclassname$ 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ajax_comment : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html;charset=utf-8";
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            context.Response.AddHeader("pragma", "no-cache");
            context.Response.AddHeader("cache-control", "");
            context.Response.CacheControl = "no-cache";
            
            SaveComment();
        }
        /// <summary>
        /// 保存评论
        /// </summary>
        public void SaveComment() 
        {
            int contentMaxLength = 1000;    //内容最长长度

            int postid = Jqpress.Framework.Web.PressRequest.GetFormInt("postid", 0);
            string author = StringHelper.CutString(Jqpress.Framework.Web.PressRequest.GetFormString("commentauthor"), 0, 20);
            string email = StringHelper.CutString(Jqpress.Framework.Web.PressRequest.GetFormString("commentemail"), 0, 50);
            string siteurl = StringHelper.CutString(Jqpress.Framework.Web.PressRequest.GetFormString("commentsiteurl"), 0, 100);
            int parentid = Jqpress.Framework.Web.PressRequest.GetFormInt("ReplyToCommentId", 0);
            string content = Jqpress.Framework.Web.PressRequest.GetFormString("commentcontent");
            int remeber = Jqpress.Framework.Web.PressRequest.GetFormInt("commentremember", 0);
            int emailnotify = Jqpress.Framework.Web.PressRequest.GetFormInt("commentemail", 0);
            string verifycode = Jqpress.Framework.Web.PressRequest.GetFormString("commentverifycode");

            CommentInfo c = new CommentInfo();

            c.ParentId = parentid;
            c.Contents = StringHelper.TextToHtml(StringHelper.CutString(content, 0, contentMaxLength));
            c.CreateTime = DateTime.Now;
            c.Email = Jqpress.Framework.Web.HttpHelper.HtmlEncode(email);
            c.EmailNotify = emailnotify;
            c.IpAddress = Jqpress.Framework.Web.PressRequest.GetIP();
            c.ParentId = 0;
            c.PostId = postid;
            c.UserId = 0;
            c.Author = author;
            c.AuthorUrl = siteurl;

            PostInfo post = PostService.GetPost(postid);
            switch (BlogConfig.GetSetting().CommentApproved)
            {
                case 1:
                    c.Approved = (int)ApprovedStatus.Success;
                    break;
                case 2:
                    string[] blackwords = BlogConfig.GetSetting().CommentSpamwords.Split(',');
                    bool hasBlackword = false;
                    foreach (string word in blackwords)
                    {
                        if (c.Contents.IndexOf(word) != -1)
                        {
                            hasBlackword = true;
                            break;
                        }
                    }
                    c.Approved = hasBlackword ? (int)ApprovedStatus.Wait : (int)ApprovedStatus.Success;
                    break;
                case 3:
                default:
                    c.Approved = (int)ApprovedStatus.Wait;
                    break;
            }


            int newID = CommentService.InsertComment(c);
            string response = "<li id=\"comment-comment-"+newID+"\" class=\"comment even thread-even depth-1\">";
            response += "<div class=\"comment-body\" id=\"div-comment-"+newID+"\">";
            response += "<div class=\"comment-author vcard\">";
            response += "<img width=\"26\" height=\"26\" class=\"avatar avatar-26 photo avatar-default\" ";
            response += "src=\"http://www.gravatar.com/avatar/" + c.GravatarCode + "?size=26\" \">";
            response += "<cite class=\"fn\">"+c.Author+"</cite> ";
            response += "<span class=\"says\">说：</span></div>";
            response += " <p> ";
            response += c.Contents;
            response += "</p>";
            response += "</li>";
            HttpContext.Current.Response.Write(response);
            SendMail(postid,email,post,content,author,siteurl,c,parentid);
            HttpContext.Current.Response.End();


            /*
             * <li id="comment-comment-${item.commentid}" class="comment even thread-even depth-1">
             <div class="comment-author vcard">
            <img width="26" height="26" class="avatar avatar-26 photo avatar-default" 
            src="http://www.gravatar.com/avatar/${item.gravatarcode}?size=26" alt="${item.nickname}">
             * 
            <cite class="fn">${item.authorlink}</cite> <span class="says">说：</span></div>
             */
        }

        public void SendMail(int postid, string email, PostInfo post, string content, string author, string siteurl, CommentInfo c,int parentid)
        {
            #region 发邮件
            try 
            {

                if (BlogConfig.GetSetting().SendMailNotifyByComment == 1) //给订阅者发邮件
                {
                    //先不考虑审核的问题
                    List<CommentInfo> list = CommentService.GetCommentList(int.MaxValue, 1, -1, postid, 0, -1, 1, string.Empty);

                    List<string> emailList = new List<string>();

                    foreach (CommentInfo cmt in list)
                    {
                        if (!Jqpress.Framework.Utils.Validate.IsValidEmail(cmt.Email))
                        {
                            continue;
                        }
                        //自己不用发
                        if (email == cmt.Email)
                        {
                            continue;
                        }
                        //不重复发送
                        if (emailList.Contains(cmt.Email))
                        {
                            continue;
                        }
                        emailList.Add(cmt.Email);

                        string subject = string.Empty;
                        string body = string.Empty;

                        subject = string.Format("[评论订阅通知]{0}", post.Title);
                        body += string.Format("您订阅的{0}有新评论了:<br/>", post.Title);
                        body += "<hr/>";
                        body += content;
                        body += "<hr/>";
                        body += "<br />评论作者: " + author;

                        if (!string.IsNullOrEmpty(siteurl))
                        {
                            body += string.Format(" (<a href=\"{0}\">{0}</a>)", siteurl);
                        }

                        body += "<br />评论时间: " + DateTime.Now;
                        body += string.Format("<br />原文连接: <a href=\"{0}\" title=\"{1}\" >{1}</a>", post.Url, post.Title);

                        body += "<br />注:系统自动通知邮件,不要回复。";

                        EmailHelper.SendAsync(cmt.Email, subject, body);
                    }
                }

                if (BlogConfig.GetSetting().SendMailAuthorByComment == 1 && c.ParentId <= 0)       //给文章作者发邮件 回复时不发邮件
                {
                    string subject = string.Empty;
                    string body = string.Empty;

                    subject = string.Format("[新评论通知]{0}", post.Title);
                    body += string.Format("您发表的{0}有新评论了:<br/>", post.Title);
                    body += "<hr/>";
                    body += content;
                    body += "<hr/>";
                    body += "<br />评论作者: " + author;

                    if (!string.IsNullOrEmpty(siteurl))
                    {
                        body += string.Format(" (<a href=\"{0}\">{0}</a>)", siteurl);
                    }

                    body += "<br />评论时间: " + DateTime.Now;
                    body += string.Format("<br />原文连接: <a href=\"{0}\" title=\"{1}\" >{1}</a>", post.Url, post.Title);

                    body += "<br />注:系统自动通知邮件,不要回复。";

                    UserInfo user = UserService.GetUser(post.UserId);
                    if (user != null && Jqpress.Framework.Utils.Validate.IsValidEmail(user.Email))
                    {
                        EmailHelper.SendAsync(user.Email, subject, body);
                    }
                }
                if (BlogConfig.GetSetting().SendMailNotifyByComment == 1)//回复时发邮件
                {
                    //获取原品论者的邮箱
                    string replyemail = CommentService.GetComment(parentid).Email;
                    string subject = string.Empty;
                    string body = string.Empty;

                    subject = string.Format("[评论回复通知]{0}", post.Title);
                    body += string.Format("您对 {0} 发表的评论有回复了:<br/>", post.Title);
                    body += "<hr/>";
                    body += content;
                    body += "<hr/>";
                    body += "<br />评论作者: " + author;

                    if (!string.IsNullOrEmpty(siteurl))
                    {
                        body += string.Format(" (<a href=\"{0}\">{0}</a>)", siteurl);
                    }

                    body += "<br />回复时间: " + DateTime.Now;
                    body += string.Format("<br />原文连接: <a href=\"{0}\" title=\"{1}\" >{1}</a>", post.Url, post.Title);

                    body += "<br />注:系统自动通知邮件,不要回复。";

                    if (email != "" && Jqpress.Framework.Utils.Validate.IsValidEmail(replyemail))
                    {
                        EmailHelper.SendAsync(email, subject, body);
                    }
                }

            }catch(Exception e){

            }
            #endregion
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
