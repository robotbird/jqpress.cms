using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Configuration;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;

namespace Jqpress.Core.Domain
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class UserInfo : IComparable<UserInfo>
    {

        public int CompareTo(UserInfo other)
        {
            return this.UserId.CompareTo(other.UserId);
        }

        #region 非字段

        private string _url;
        private string _link;

        /// <summary>
        /// 地址
        /// </summary>
        public string Url
        {
            //set { _url = value; }
            //get { return _url; }

            get { return ConfigHelper.SiteUrl + "author/" + Jqpress.Framework.Web.HttpHelper.UrlEncode(this.UserName) + SiteConfig.GetSetting().RewriteExtension; }
        }

        /// <summary>
        /// 连接
        /// </summary>
        public string Link
        {
            //set { _link = value; }
            //get { return _link; }

            get { return string.Format("<a href=\"{0}\" title=\"作者:{1}\">{1}</a>", this.Url, this.NickName); }
        }
        /// <summary>
        /// 角色显示
        /// </summary>
        public string RoleDisplay 
        {
            
            get 
            {
                string display = "";
                switch (Role) 
                {
                    case 1:display = "管理员"; break;
                    case 2: display = "编辑"; break;
                    case 3: display = "订阅者"; break;
                    case 4: display = "投稿者"; break;
                    case 5: display = "作者"; break;
                    default: display = "管理员"; break;
                }
                return display;
            }
        }

        #endregion


        /// <summary>
        ///  用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户角色 
        /// </summary>
        public int Role { get; set; }



        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 个人网站
        /// </summary>
        public string SiteUrl { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 用户状态
        /// 1:使用,0:停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 统计日志数
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
