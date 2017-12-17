using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;
using Jqpress.Core.Repositories.IRepository;
using Jqpress.Core.Repositories.Repository;

namespace Jqpress.Core.Services
{
    public class StatisticsService
    {
        private ISiteRepository _siteRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public StatisticsService()
            : this(new SiteRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="siteRepository"></param>
        public StatisticsService(ISiteRepository siteRepository)
        {
            this._siteRepository = siteRepository;
        }
        #endregion

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public  StatisticsInfo GetStatistics()
        {
            return _siteRepository.GetStatistics();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public  bool UpdateStatistics(StatisticsInfo statistics)
        {
            return _siteRepository.UpdateStatistics(statistics);
        }

        /// <summary>
        /// 更新文章数
        /// </summary>
        /// <param name="addCount">增加数，可为负数</param>
        /// <returns></returns>
        public  bool UpdateStatisticsPostCount(int addCount)
        {
            var statistic = _siteRepository.GetStatistics();
            statistic.PostCount += addCount;
            return UpdateStatistics(statistic);
        }

        /// <summary>
        /// 更新评论数
        /// </summary>
        /// <param name="addCount">增加数，可为负数</param>
        /// <returns></returns>
        public  bool UpdateStatisticsCommentCount(int addCount)
        {
            var statistic = _siteRepository.GetStatistics();
            statistic.CommentCount += addCount;
            return UpdateStatistics(statistic);
        }
    }
}
