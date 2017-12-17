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
        /// 更新统计
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        bool UpdateStatistics(StatisticsInfo statistics);

        /// <summary>
        /// 获取统计
        /// </summary>
        /// <returns></returns>
        StatisticsInfo GetStatistics();

    }
}
