using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 博客配置类
    /// </summary>
    public class SiteConfigInfo
    {
        private string _sitename = "机器鸟";
        private string _sitedescription = "";
        private string _metakeywords;
        private string _metadescription;
        private int _sitestatus = 1;
        private int _sitetotaltype = 1;
        private int _enableverifycode = 1;


        private int _commentstatus = 1;
        private int _commentorder = 0;
        private int _commentapproved = 1;
        private string _commentblackword = "";

        private int _sidebarpostcount = 10;
        private int _sidebarcommentcount = 10;
        private int _sidebartagcount = 20;

        private int _postshowtype = 2;
        private int _postrelatedcount = 5;



        private int _rssstatus = 1;
        private int _rssrowcount = 20;
        private int _rssshowtype = 2;

        private int _pagesizepostcount = 10;
        private int _pagesizecommentcount = 50;
        //     private int _pagesizetagcount = 10;

        private int _urlformattype = 2;
        private string _rewriteextension = ".aspx";

        private string _footerhtml;

        private string _theme = "default";
        private string _mobiletheme = "mobile";

        private int _watermarktype = 1;
        private int _watermarkposition = 4;
        private int _watermarktransparency = 8;
        private int _watermarkquality = 80;
        private string _watermarktext = "机器鸟";
        private int _watermarkfontsize = 14;
        private string _watermarkfontname = "Tahoma";
        private string _watermarkimage = "watermark.gif";

        private string _smtpemail = "yourname@gmail.com";
        private string _smtpserver = "smtp.gmail.com";
        private int _smtpserverpost = 25;
        private string _smtpusername = "yourname";
        private string _smtppassword = "yourpassword";
        private int _smtpenablessl = 1;

        private int _sendmailauthorbypost = 0;
        private int _sendmailauthorbycomment = 0;
        private int _sendmailnotifybycomment = 1;

        #region 邮件发送设置
        /// <summary>
        /// 发表新文章时给所有作者发一封邮件
        /// </summary>
        public int SendMailAuthorByPost
        {
            set { _sendmailauthorbypost = value; }
            get { return _sendmailauthorbypost; }
        }
        /// <summary>
        /// 发布新评论时给文章作者发一封邮件
        /// </summary>
        public int SendMailAuthorByComment
        {
            set { _sendmailauthorbycomment = value; }
            get { return _sendmailauthorbycomment; }
        }
        /// <summary>
        /// 发布新评论时给该文章评论订阅者发一封邮件
        /// </summary>
        public int SendMailNotifyByComment
        {
            set { _sendmailnotifybycomment = value; }
            get { return _sendmailnotifybycomment; }
        }

        #endregion

        #region 邮件设置
        /// <summary>
        /// 邮箱
        /// </summary>
        public string SmtpEmail
        {
            set { _smtpemail = value; }
            get { return _smtpemail; }
        }

        /// <summary>
        /// 服务器
        /// </summary>
        public string SmtpServer
        {
            set { _smtpserver = value; }
            get { return _smtpserver; }
        }
        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpServerPost
        {
            set { _smtpserverpost = value; }
            get { return _smtpserverpost; }
        }
        /// <summary>
        /// 帐号
        /// </summary>
        public string SmtpUserName
        {
            set { _smtpusername = value; }
            get { return _smtpusername; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string SmtpPassword
        {
            set { _smtppassword = value; }
            get { return _smtppassword; }
        }
        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public int SmtpEnableSsl
        {
            set { _smtpenablessl = value; }
            get { return _smtpenablessl; }
        }
        #endregion

        #region 全局
        /// <summary>
        /// 网站名称
        /// </summary>
        public string SiteName
        {
            set { _sitename = value; }
            get { return _sitename; }
        }

        /// <summary>
        /// 网站描述
        /// </summary>
        public string SiteDescription
        {
            set { _sitedescription = value; }
            get { return _sitedescription; }
        }



        /// <summary>
        /// Meta 关键字
        /// </summary>
        public string MetaKeywords
        {
            set { _metakeywords = value; }
            get { return _metakeywords; }
        }

        /// <summary>
        /// Meta 描述
        /// </summary>
        public string MetaDescription
        {
            set { _metadescription = value; }
            get { return _metadescription; }
        }

        /// <summary>
        /// 网站状态
        /// </summary>
        public int SiteStatus
        {
            set { _sitestatus = value; }
            get { return _sitestatus; }
        }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {

            get { return "1.0"; }
        }
        /// <summary>
        /// 程序集版本
        /// </summary>
        //  public const string AssemblyVersion = "0.5.0";


        /// <summary>
        /// 统计类型(包括文章浏览次数,网站访问次数)
        /// 1:按刷新次数统计,2:按IP 24小时统计一次
        /// </summary>
        public int SiteTotalType
        {
            set { _sitetotaltype = value; }
            get { return _sitetotaltype; }
        }

        /// <summary>
        /// 启用验证码
        /// </summary>
        public int EnableVerifyCode
        {
            set { _enableverifycode = value; }
            get { return _enableverifycode; }
        }
        /// <summary>
        /// 是否已安装
        /// </summary>
        public bool IsInstalled { get; set; }

        #endregion

        #region 评论相关
        /// <summary>
        /// 允许评论
        /// </summary>
        public int CommentStatus
        {
            set { _commentstatus = value; }
            get { return _commentstatus; }
        }

        /// <summary>
        /// 评论排序
        /// 0:顺序,1:倒序
        /// </summary>
        public int CommentOrder
        {
            set { _commentorder = value; }
            get { return _commentorder; }
        }

        /// <summary>
        /// 评论审核 
        /// 1:不审核,2:自动审核,3:人工审核
        /// </summary>
        public int CommentApproved
        {
            set { _commentapproved = value; }
            get { return _commentapproved; }
        }

        /// <summary>
        /// 评论垃圾词汇
        /// </summary>
        public string CommentSpamwords
        {
            set { _commentblackword = value; }
            get { return _commentblackword; }
        }
        #endregion

        #region Rss相关

        /// <summary>
        /// Rss状态
        /// 1:启用,0:停用
        /// </summary>
        public int RssStatus
        {
            set { _rssstatus = value; }
            get { return _rssstatus; }
        }

        /// <summary>
        /// Rss输入行数
        /// </summary>
        public int RssRowCount
        {
            set { _rssrowcount = value; }
            get { return _rssrowcount; }
        }

        /// <summary>
        /// Rss输入类型
        /// 1:仅标题,2:摘要,3,正文前200字,4:正文
        /// </summary>
        public int RssShowType
        {
            set { _rssshowtype = value; }
            get { return _rssshowtype; }
        }

        #endregion

        #region 侧栏

        /// <summary>
        /// 侧栏文章数
        /// </summary>
        public int SidebarPostCount
        {
            set { _sidebarpostcount = value; }
            get { return _sidebarpostcount; }
        }

        /// <summary>
        /// 评论数
        /// </summary>
        public int SidebarCommentCount
        {
            set { _sidebarcommentcount = value; }
            get { return _sidebarcommentcount; }
        }

        /// <summary>
        /// 标签数
        /// </summary>
        public int SidebarTagCount
        {
            set { _sidebartagcount = value; }
            get { return _sidebartagcount; }
        }



        #endregion

        #region 分页相关

        /// <summary>
        /// 文章数
        /// </summary>
        public int PageSizePostCount
        {
            set { _pagesizepostcount = value; }
            get { return _pagesizepostcount; }
        }


        /// <summary>
        /// 评论数
        /// </summary>
        public int PageSizeCommentCount
        {
            set { _pagesizecommentcount = value; }
            get { return _pagesizecommentcount; }
        }


        ///// <summary>
        ///// 标签数
        ///// </summary>
        //public int PageSizeTagCount
        //{
        //    set { _pagesizetagcount = value; }
        //    get { return _pagesizetagcount; }
        //}

        #endregion

        #region 重写

        /// <summary>
        /// Url格式化类型
        /// </summary>
        public int UrlFormatType
        {
            set { _urlformattype = value; }
            get { return _urlformattype; }
        }


        /// <summary>
        /// 重写扩展名
        /// </summary>
        public string RewriteExtension
        {
            set { _rewriteextension = value; }
            get { return _rewriteextension; }
        }

        #endregion

        #region 页脚

        /// <summary>
        /// 页脚Html
        /// </summary>
        public string FooterHtml
        {
            set { _footerhtml = value; }
            get { return _footerhtml; }
        }

        #endregion

        #region 主题

        /// <summary>
        /// 主题
        /// </summary>
        public string Theme
        {
            set { _theme = value; }
            get { return _theme; }
        }

        /// <summary>
        /// 手机版主题
        /// </summary>
        public string MobileTheme
        {
            set { _mobiletheme = value; }
            get { return _mobiletheme; }
        }

        #endregion

        #region 水印

        /// <summary>
        /// 水印类型 1:文字,2:图片
        /// </summary>
        public int WatermarkType
        {
            set { _watermarktype = value; }
            get { return _watermarktype; }
        }


        /// <summary>
        /// 水印定位 1:左上,2:左下,3:右上,4:右下,5:中心
        /// </summary>
        public int WatermarkPosition
        {
            set { _watermarkposition = value; }
            get { return _watermarkposition; }
        }


        /// <summary>
        /// 水印透明度 1-10,10为不透明
        /// </summary>
        public int WatermarkTransparency
        {
            set { _watermarktransparency = value; }
            get { return _watermarktransparency; }
        }


        /// <summary>
        /// 图片质量,0-100,100 为最高
        /// </summary>
        public int WatermarkQuality
        {
            set { _watermarkquality = value; }
            get { return _watermarkquality; }
        }


        /// <summary>
        /// 水印文字
        /// </summary>
        public string WatermarkText
        {
            set { _watermarktext = value; }
            get { return _watermarktext; }
        }


        /// <summary>
        /// 水印文字大小
        /// </summary>
        public int WatermarkFontSize
        {
            set { _watermarkfontsize = value; }
            get { return _watermarkfontsize; }
        }


        /// <summary>
        /// 水印文字字体
        /// </summary>
        public string WatermarkFontName
        {
            set { _watermarkfontname = value; }
            get { return _watermarkfontname; }
        }


        /// <summary>
        /// 水印图片名
        /// </summary>
        public string WatermarkImage
        {
            set { _watermarkimage = value; }
            get { return _watermarkimage; }
        }
        #endregion

        #region 文章

        /// <summary>
        /// 相关文章数
        /// </summary>
        public int PostRelatedCount
        {
            set { _postrelatedcount = value; }
            get { return _postrelatedcount; }
        }

        /// <summary>
        /// 文章显示类型
        /// 1:仅标题,2:摘要,3,正文前200字,4:正文
        /// </summary>
        public int PostShowType
        {
            set { _postshowtype = value; }
            get { return _postshowtype; }
        }
        #endregion
    }
}
