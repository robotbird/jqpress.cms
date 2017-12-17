using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;
namespace Jqpress.Blog.Data.IData
{
    public partial interface IDataProvider
    {
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int InsertTag(TagInfo tag);

        /// <summary>
        /// 修改标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int UpdateTag(TagInfo tag);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        int DeleteTag(int tagId);

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        TagInfo GetTag(int tagId);

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns></returns>
        List<TagInfo> GetTagList();

    }
}
