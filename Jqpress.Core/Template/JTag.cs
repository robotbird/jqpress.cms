using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Web;
using Jqpress.Core.Domain;
using Jqpress.Core.Services;
using Jqpress.Core.Configuration;


namespace Jqpress.Core.Template
{
   public class JTag
    {
       /// <summary>
       /// 分类
       /// </summary>
       public class Cate 
       {
           /// <summary>
           /// 分类id
           /// </summary>
           public int  Id { get; set; }
           /// <summary>
           /// 分类简地址
           /// </summary>
           public string pagename { get; set; }

           public CategoryInfo Page
           {
               get
               {
                   //TODO:考虑用单例模式实例化
                   var cate = new CategoryService();
                   if (!string.IsNullOrEmpty(this.pagename)) 
                   {
                       return cate.GetCategory(this.pagename);                   
                   }
                   else if (this.Id>0)
                   {
                       return cate.GetCategory(this.Id);
                   }
                   else {
                       return new CategoryInfo();
                   }
               }
           }
       }
       /// <summary>
       /// 内容
       /// </summary>
       public class Post
       {
           /// <summary>
           /// 获取数量
           /// </summary>
           public int num { get; set; }
           /// <summary>
           /// 分类id
           /// </summary>
           public dynamic CategoryId { get; set; }
           /// <summary>
           /// 分类pagename
           /// </summary>
           public string pagename { get; set; }
           /// <summary>
           /// 是否首页显示
           /// </summary>
           public int HomeStatus { get; set; }
           /// <summary>
           /// 列表
           /// </summary>
           public List<PostInfo> List 
           {
               get 
               {
                   var postService = new PostService();
                   var cateService = new CategoryService();
                   var cateids = "";
                   if (!string.IsNullOrEmpty(this.pagename)) 
                   {
                       CategoryInfo cate = cateService.GetCategory(this.pagename);
                       int categoryId = cate.CategoryId;
                       cateids = categoryId + "," + cateService.GetCategoryList().FindAll(c => c.ParentId == categoryId).Aggregate(string.Empty, (current, t) => current + (t.CategoryId + ",")).TrimEnd(',');
                       cateids = cateids.TrimEnd(',');
                   }
                   
                   var list =  postService.GetPostList();
                   if(cateids=="")
                   {
                       list.Take(num).ToList();
                   }
                   else if (cateids.IndexOf(",") < 0&&cateids!="")
                   {
                       list =list.FindAll(post => post.CategoryId == Convert.ToInt32(cateids)).Take(num).ToList();
                   }
                   else 
                   {
                       list = postService.GetPostList().FindAll(post => (cateids.Split(',').Contains(post.CategoryId.ToString()))).Take(num).ToList();                   
                   }
                   if (HomeStatus == 1) 
                   {
                       list = list.FindAll(post => post.HomeStatus == 1).OrderBy(post=>post.SortNum).ToList();
                   }

                   return list;
               }
           }
       }
       /// <summary>
       /// 全局标签
       /// </summary>
       public class Site 
       {
           /// <summary>
           /// 站点路径
           /// </summary>
           public string SiteUrl { get; set; }
           /// <summary>
           /// 站点路径
           /// </summary>
           public string SitePath { get; set; }
           /// <summary>
           /// 主题名称
           /// </summary>
           public string ThemeName { get; set; }
           /// <summary>
           /// 样式路径
           /// </summary>
           public string ThemeUrl { get; set; }
           /// <summary>
           /// 样式虚拟目录
           /// </summary>
           public string ThemePath { get; set; }
           /// <summary>
           /// 站点描述
           /// </summary>
           public string SiteDescription { get; set; }
           /// <summary>
           /// 站点名称
           /// </summary>
           public string SiteName { get; set; }

           /// 是否首页
           /// </summary>
           public int IsDefault { get; set; }
           /// <summary>
           /// 是否post
           /// </summary>
           public int IsPost { get; set; }
           /// <summary>
           /// rss路径
           /// </summary>
           public string FeedUrl { get; set; }
           /// <summary>
           /// 评论rss路径
           /// </summary>
           public string FeedCommentUrl { get; set; }

           /// <summary>
           /// 当前url
           /// </summary>
           public string Url { get; set; }
           /// <summary>
           /// 当前日期
           /// </summary>
           public string Date { get; set; }

           private string _searchkeyword;
           /// <summary>
           /// 搜索关键词
           /// </summary>
           public string SearchKeyword
           {
               get { return _searchkeyword ?? string.Empty; ; }
               set { _searchkeyword = value; }
           }

           #region 头部
           /// <summary>
           /// 标题
           /// </summary>
           public string PageTitle { get; set; }


           /// <summary>
           /// 关键词
           /// </summary>
           private string _metaKeywords;
           public string MetaKeywords
           {
               get { return _metaKeywords ?? (_metaKeywords = ""); }
               set { _metaKeywords = value; }
           }


           /// <summary>
           /// 描述
           /// </summary>
           private string _metaDescription;
           public string MetaDescription
           {
               get { return _metaDescription ?? (_metaDescription = ""); }
               set { _metaDescription = value; }
           }
           /// <summary>
           /// 头部wlwmanifest
           /// </summary>
           public string Head
           {
               get
               {
                   string headhtml = string.Empty;

                   headhtml += string.Format("<meta name=\"generator\" content=\"jqpress {0}\" />\n", "");
                   headhtml += "<meta name=\"author\" content=\"jqpress Team\" />\n";
                   headhtml += string.Format("<meta name=\"copyright\" content=\"2011-{0} jqpress Team.\" />\n", DateTime.Now.Year);
                   headhtml += string.Format("<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{0}\"  href=\"{1}\"  />\n", "");
                   headhtml += string.Format("<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"{0}xmlrpc/rsd.aspx\" />\n", "");
                   headhtml += string.Format("<link rel=\"wlwmanifest\" type=\"application/wlwmanifest+xml\" href=\"{0}xmlrpc/wlwmanifest.aspx\" />", "");

                   return headhtml;
               }
           }
           /// <summary>
           /// 头部导航
           /// </summary>
           public List<CategoryInfo> HeadNav 
           {
               get 
               {
                   //TODO:考虑用单例模式实例化
                   var cateService = new CategoryService();
                   var list = cateService.GetCategoryList();
                   return list;
               } 
           }
           #endregion

           #region 底部
           /// <summary>
           /// 底部版权信息
           /// </summary>
           public string FooterHtml { get; set; }
           /// <summary>
           /// 版本
           /// </summary>
           public string Version { get; set; }
           #endregion

           public Site() 
           {
               var theme = PressRequest.GetQueryString("theme");
               if (!string.IsNullOrEmpty(theme))
               {
                   this.ThemeName = theme;
               }
               else 
               {
                   this.ThemeName = SiteConfig.GetSetting().Theme;               
               }
               this.SiteName = SiteConfig.GetSetting().SiteName;
               this.SiteUrl = ConfigHelper.SiteUrl;
               this.MetaKeywords = SiteConfig.GetSetting().MetaKeywords;
               this.MetaDescription = SiteConfig.GetSetting().MetaDescription;
               this.SiteDescription = SiteConfig.GetSetting().SiteDescription;
               this.FooterHtml = SiteConfig.GetSetting().FooterHtml;
           }
       }
    }
}
