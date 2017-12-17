using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;


namespace Jqpress.Core.Repositories.IRepository
{
    public partial interface ICategoryRepository : IRepository<CategoryInfo>
    {
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        CategoryInfo GetCategoryByPageName(string pagename);
    }
}
