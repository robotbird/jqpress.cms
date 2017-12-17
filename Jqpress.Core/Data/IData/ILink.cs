using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;

namespace Jqpress.Blog.Data.IData
{
    partial interface IDataProvider
    {
        /// <summary>
        /// 添加连接
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        int InsertLink(LinkInfo link);

        /// <summary>
        /// 修改连接
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        int UpdateLink(LinkInfo link);

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
        int DeleteLink(int linkId);

        //        LinkInfo GetLink(int linkId);

        /// <summary>
        /// 获取全部连接
        /// </summary>
        /// <returns></returns>
        List<LinkInfo> GetLinkList();
    }
}
