using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jqpress.Core.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Services;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.DbProvider.Access;
using Jqpress.Framework.Utils;

namespace Jqpress.Core.Common
{
    public class BasePage : System.Web.UI.Page
    {
    /// <summary>
    /// 查询次数统计
    /// </summary>
    public static int querycount = 0;

    public static string QueryDetailStr = "";
#if DEBUG
    public static string querydetail = "";
#endif


    /// <summary>
    /// 当前页面开始载入时间(毫秒)
    /// </summary>
    public static DateTime starttick;

    public BasePage()
    {
        starttick = DateTime.Now;

        querycount =Jqpress.Framework.DbProvider.Access.OleDbHelper.QueryCount;
        Jqpress.Framework.DbProvider.Access.OleDbHelper.QueryCount = 0;

    }
    /// <summary>
    /// sql分析代码
    /// </summary>
    public static string SqlAnalytical
    {
        get
        {
            //清空当前页面查询统计
#if DEBUG
            //      SqlHelper.QueryCount = 0;
            //    SqlHelper.QueryDetail = "";
#endif
            //清空当前页面查询统计
#if DEBUG
            //   querydetail = SqlHelper.QueryDetail;
            //    SqlHelper.QueryDetail = "";
#endif

            querycount = OleDbHelper.QueryCount;
            OleDbHelper.QueryCount = 0;

#if DEBUG
            querydetail = OleDbHelper.QueryDetail;
            OleDbHelper.QueryDetail = "";
#endif
            string analyticalstr = "";
#if DEBUG
            analyticalstr = "<div style=\"margin:0px auto\">注意: 以下为debug数据查询分析工具，正式站点使用Release编译。</div><div style=margin:0px auto>查询次数：" + querycount + "</div>" + querydetail + "";
#endif

            return analyticalstr;
        }
    }
    /// <summary>
    /// 输出提示,并终止页面
    /// </summary>
    /// <param name="title"></param>
    /// <param name="msg"></param>
    public static void ResponseError(string title, string msg)
    {
        string str = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><header><title>{0} - {1}</title><style>a {{ color:gray;}}</style></header><body><div  style=\"border:1px solid #94a2a3; background-color:#F1F8FE; width:500px;   padding:8px 25px; margin:100px auto 0 auto;\" ><h5>提示:</h5><h2>{2}</h2><h4><a href=\"{3}\">返回首页</a></h4></div><div  style=\"  text-align:center; padding:5px 0;color:gray;font-size:12px;\" >Powered by <a href=\"http://www.jqpress.com\" target=\"_blank\">jqpress</a> </div></body></html>";

        //str = string.Format(str, title, SettingManager.GetSetting().SiteName, msg, ConfigHelper.SiteUrl);

        //HttpContext.Current.Response.Clear();
        //HttpContext.Current.Response.Write(str);
        //HttpContext.Current.Response.End();

        ResponseError(title, msg, 500);

    }

    public static void ResponseError(string title, string msg, int statusCode)
    {
        string str = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><header><title>{0} - {1}</title><style>a {{ color:gray;}}</style></header><body><div  style=\"border:1px solid #94a2a3; background-color:#F1F8FE; width:500px;   padding:8px 25px; margin:100px auto 0 auto;\" ><h5>提示:</h5><h2>{2}</h2><h4><a href=\"{3}\">返回首页</a></h4></div><div  style=\"  text-align:center; padding:5px 0;color:gray;font-size:12px;\" >Powered by <a href=\"http://www.jqpress.com\" target=\"_blank\">jqpress</a> </div></body></html>";

        str = string.Format(str, title, SiteConfig.GetSetting().SiteName, msg, ConfigHelper.SiteUrl);

        HttpContext.Current.Response.Clear();
      //  HttpContext.Current.Response.Status = "200 Internal Server Error";
        HttpContext.Current.Response.StatusCode = statusCode;
        HttpContext.Current.Response.Write(str);
        HttpContext.Current.Response.End();
    }
    #region 用户COOKIE操作
    /// <summary>
    /// 用户COOKIE名
    /// </summary>
    private static readonly string CookieUser = ConfigHelper.SitePrefix + "admin";
    /// <summary>
    /// 读当前用户COOKIE
    /// </summary>
    /// <returns></returns>
    public static HttpCookie ReadUserCookie()
    {
        return HttpContext.Current.Request.Cookies[CookieUser];
    }

    /// <summary>
    /// 移除当前用户COOKIE
    /// </summary>
    /// <returns></returns>
    public static bool RemoveUserCookie()
    {
        HttpCookie cookie = new HttpCookie(CookieUser);
        cookie.Values.Clear();
        cookie.Expires = DateTime.Now.AddYears(-1);

        HttpContext.Current.Response.AppendCookie(cookie);
        return true;
    }

    /// <summary>
    /// 写/改当前用户COOKIE
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="expires"></param>
    /// <returns></returns>
    public static bool WriteUserCookie(int userID, string userName, string password, int expires)
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieUser];
        if (cookie == null)
        {
            cookie = new HttpCookie(CookieUser);
        }
        if (expires > 0)
        {
            cookie.Values["expires"] = expires.ToString();
            cookie.Expires = DateTime.Now.AddMinutes(expires);
        }
        else
        {
            int temp_expires = Convert.ToInt32(cookie.Values["expires"]);
            if (temp_expires > 0)
            {
                cookie.Expires = DateTime.Now.AddMinutes(temp_expires);
            }
        }
        cookie.Values["userid"] = userID.ToString();
        cookie.Values["username"] = HttpContext.Current.Server.UrlEncode(userName);
        cookie.Values["key"] = Jqpress.Framework.Utils.EncryptHelper.MD5(userID + HttpContext.Current.Server.UrlEncode(userName) + password);

        HttpContext.Current.Response.AppendCookie(cookie);
        return true;
    }
    #endregion

    /// <summary>
    /// 验证码
    /// </summary>
    /// <returns></returns>
    public static string VerifyCode
    {
        get
        {
            return Convert.ToString(HttpContext.Current.Session[ConfigHelper.SitePrefix + "VerifyCode"]);
        }
        set
        {
            HttpContext.Current.Session[ConfigHelper.SitePrefix + "VerifyCode"] = value;
        }

    }
    /// <summary>
    /// 是否登陆
    /// </summary>
    public static bool IsLogin
    {
        get
        {
            HttpCookie c = ReadUserCookie();

            if (c != null)
            {
                return Jqpress.Framework.Utils.TypeConverter.StrToInt(c["userid"], 0) > 0;
            }
            return false;
        }
    }
    /// <summary>
    /// 登陆用户ID
    /// </summary>
    public static int CurrentUserId
    {
        get
        {

            if (IsLogin)
            {
                return Jqpress.Framework.Utils.TypeConverter.StrToInt(ReadUserCookie()["userid"], 0);
            }
            return 0;
        }
    }

    /// <summary>
    /// 登陆名
    /// </summary>
    public static string CurrentUserName
    {
        get
        {
            if (IsLogin)
            {
                return HttpContext.Current.Server.UrlDecode(ReadUserCookie()["username"]);
            }
            return string.Empty;
        }
    }



    /// <summary>
    /// 当前Key
    /// </summary>
    public static string CurrentKey
    {
        get
        {
            if (IsLogin)
            {
                return ReadUserCookie()["key"];
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// 当前用户
    /// </summary>
    public static UserInfo CurrentUser
    {
        get
        {
            if (IsLogin)
            {
                return (new UserService()).GetUser(CurrentUserId);
            }
            return null;
        }
    }

    /// <summary>
    /// 错误处理
    /// </summary>
    /// <param name="e"></param>
    protected override void OnError(EventArgs e)
    {
        Exception ex = Server.GetLastError();
        if (ex != null)
        {
            ResponseError("空间比较差劲啊,又出问题了", ex.Message);
            Server.ClearError();
        }
        base.OnError(e);

    }
    }
}
