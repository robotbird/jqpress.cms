using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;

namespace Jqpress.Core.Repositories.IRepository
{
    public partial interface ISiteRepository
    {
        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        StatisticsInfo GetStatistics();
        /// <summary>
        /// 更新统计数据
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        bool UpdateStatistics(StatisticsInfo statistics);
    }
}
