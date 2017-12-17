using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;

namespace Jqpress.Core.Repositories.IRepository
{
    public partial interface ITagRepository : IRepository<TagInfo>
    {
       /// <summary>
        /// 根据名称获取标签
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TagInfo GetTagByName(string name);

        /// <summary>
        /// 根据pagename获取标签
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        TagInfo GetTagByPageName(string pagename);
    }
}
