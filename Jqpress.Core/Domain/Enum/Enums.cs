using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Core.Domain.Enum
{
    /// <summary>
    /// 分类类别
    /// </summary>
    public enum CategoryType
    {
        Category = 1,
        Tag = 2,
        //Catalog = 1,
        //Keyword = 4,
        //Tags = 2,
        Unknown = 0

    }

    /// <summary>
    /// 连接类型(后续版本用)
    /// </summary>
    public enum LinkType
    {
        Unknow = 0,

        /// <summary>
        /// 系统
        /// </summary>
        System = 1,

        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 2,
    }


    /// <summary>
    /// 连接位置
    /// </summary>
    public enum LinkPosition
    {
        Unknow = 0,

        /// <summary>
        /// 导航
        /// </summary>
        Navigation = 1,

        /// <summary>
        /// 普通
        /// </summary>
        General = 2,
    }



    /// <summary>
    /// 内容状态
    /// </summary>
    public enum PostStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 0,

        /// <summary>
        /// 发布
        /// </summary>
        Published = 1,


        //Top,
        //Hide,
    }

    /// <summary>
    /// 审核状态
    /// </summary>
    public enum ApprovedStatus
    {
        /// <summary>
        /// 未审核
        /// </summary>
        Wait = 0,

        /// <summary>
        /// 已通过
        /// </summary>
        Success = 1,

    }

    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 管理员
        /// </summary>
        Administrator = 1,
        /// <summary>
        /// 编辑
        /// </summary>
        Editor = 2,
        /// <summary>
        /// 订阅者
        /// </summary>
        Subscriber =3,
        /// <summary>
        /// 投稿者
        /// </summary>
        Contributor = 4,
        /// <summary>
        /// 作者
        /// </summary>
        Author = 5,
    }

    /// <summary>
    /// 文章URL格式
    /// </summary>
    public enum PostUrlFormat
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 日期
        /// </summary>
        Date = 1,

        /// <summary>
        /// 别名
        /// </summary>
        PageName = 2,

        ///// <summary>
        ///// 分类
        ///// </summary>
        //Category = 3,
    }
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperateType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Insert = 0,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
    }
    public enum NotifyType
    {
        Success,
        Error
    }
    /// <summary>
    /// 栏目类型
    /// </summary>
    public enum CateType 
    {
        /// <summary>
        /// 产品列表
        /// </summary>
        ProList =1,
        /// <summary>
        /// 普通信息列表
        /// </summary>
        ItemList =2,
        /// <summary>
        /// 单页面
        /// </summary>
        Page = 3
    }
}
