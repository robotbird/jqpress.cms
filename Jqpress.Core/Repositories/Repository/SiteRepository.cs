using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.DbProvider;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Repositories.Repository
{
   public partial class SiteRepository:ISiteRepository
    {
       

       /// <summary>
       /// 更新统计数据
       /// </summary>
       /// <param name="statistics"></param>
       /// <returns></returns>
        public bool UpdateStatistics(StatisticsInfo statistics)
        {
            string cmdText = string.Format("update [{0}sites] set PostCount=@PostCount,CommentCount=@CommentCount,VisitCount=@VisitCount,TagCount=@TagCount", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new
                {
                    PostCount = statistics.PostCount,
                    CommentCount = statistics.CommentCount,
                    VisitCount = statistics.VisitCount,
                    TagCount = statistics.TagCount
                })>0;
            }
        }
       /// <summary>
       /// 获取统计数据
       /// </summary>
       /// <returns></returns>
        public StatisticsInfo GetStatistics()
        {
            //string cmdText = string.Format("select top 1 * from [{0}sites]", ConfigHelper.Tableprefix);
            string cmdText = string.Format("select * from [{0}sites] limit 1", ConfigHelper.Tableprefix);//for sqlite
            string insertText = string.Format("insert into [{0}sites] ([PostCount],[CommentCount],[VisitCount],[TagCount]) values ( '0','0','0','0')", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
               var list = conn.Query<StatisticsInfo>(cmdText,null).ToList();
               if (list.Count == 0)
               {
                   conn.Execute(insertText,null);
               }
               return list.Count > 0 ? list[0] : null;
            }
        }
    }
}
