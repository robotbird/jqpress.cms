using System;
using System.Collections.Generic;
using Jqpress.Blog.Entity;
using System.Data.OleDb;
using System.Data;
using Jqpress.Framework.DbProvider.Access;
using Jqpress.Framework.Configuration;
namespace Jqpress.Blog.Data.Sqlite
{
    public partial class DataProvider
    {
        public bool UpdateStatistics(StatisticsInfo statistics)
        {
            string cmdText =string.Format( "update [{0}sites] set PostCount=@PostCount,CommentCount=@CommentCount,VisitCount=@VisitCount,TagCount=@TagCount",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = {
					                        OleDbHelper.MakeInParam("@PostCount", OleDbType.Integer,4,statistics.PostCount),
					                        OleDbHelper.MakeInParam("@CommentCount", OleDbType.Integer,4,statistics.CommentCount),
					                        OleDbHelper.MakeInParam("@VisitCount", OleDbType.Integer,4,statistics.VisitCount),
					                        OleDbHelper.MakeInParam("@TagCount", OleDbType.Integer,4,statistics.TagCount),
                                        };

            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams) == 1;
        }

        public StatisticsInfo GetStatistics()
        {
            string cmdText = string.Format("select top 1 * from [{0}sites]", ConfigHelper.Tableprefix);

            string insertText = string.Format("insert into [{0}sites] ([PostCount],[CommentCount],[VisitCount],[TagCount]) values ( '0','0','0','0')", ConfigHelper.Tableprefix);

            List<StatisticsInfo> list = DataReaderToListSite(OleDbHelper.ExecuteReader(cmdText));

            if (list.Count == 0)
            {
                OleDbHelper.ExecuteNonQuery(insertText);
            }
            list = DataReaderToListSite(OleDbHelper.ExecuteReader(cmdText));

            return list.Count > 0 ? list[0] : null;
        }
        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param name="read">OleDbDataReader</param>
        /// <returns>TermInfo</returns>
        private static List<StatisticsInfo> DataReaderToListSite(OleDbDataReader read)
        {
            var list = new List<StatisticsInfo>();
            while (read.Read())
            {
                var site = new StatisticsInfo
                               {
                                   PostCount = Convert.ToInt32(read["PostCount"]),
                                   CommentCount = Convert.ToInt32(read["CommentCount"]),
                                   VisitCount = Convert.ToInt32(read["VisitCount"]),
                                   TagCount = Convert.ToInt32(read["TagCount"])
                               };

                list.Add(site);
            }
            read.Close();
            return list;
        }


    }
}
