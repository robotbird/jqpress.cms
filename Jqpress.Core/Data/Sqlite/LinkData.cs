using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using Jqpress.Blog.Entity;
using Jqpress.Framework.DbProvider.Access;
using Jqpress.Framework.Configuration;

namespace Jqpress.Blog.Data.Sqlite
{
    public partial class DataProvider
    {
        public int InsertLink(LinkInfo link)
        {
            string cmdText = string.Format(@"insert into [{0}links]
                            (
                            [type],[linkname],[linkurl],[position],[target],[description],[sortnum],[status],[createtime]
                            )
                            values
                            (
                            @type,@linkname,@linkurl,@position,@target,@description,@sortnum,@status,@createtime
                            )",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                OleDbHelper.MakeInParam("@type",OleDbType.Integer,4,link.Type),
								OleDbHelper.MakeInParam("@linkname",OleDbType.VarWChar,100,link.LinkName),
                                OleDbHelper.MakeInParam("@linkurl",OleDbType.VarWChar,255,link.LinkUrl),
                                OleDbHelper.MakeInParam("@position",OleDbType.Integer,4,link.Position),
                                OleDbHelper.MakeInParam("@target",OleDbType.VarWChar,50,link.Target),
								OleDbHelper.MakeInParam("@description",OleDbType.VarWChar,255,link.Description),
                                OleDbHelper.MakeInParam("@sortnum",OleDbType.Integer,4,link.SortNum),
								OleDbHelper.MakeInParam("@status",OleDbType.Integer,4,link.Status),
								OleDbHelper.MakeInParam("@createtime",OleDbType.Date,8,link.CreateTime),
							};

            int r = OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
            if (r > 0)
            {
                return Convert.ToInt32(OleDbHelper.ExecuteScalar(string.Format("select top 1 [linkid] from [{0}links]  order by [linkid] desc",ConfigHelper.Tableprefix)));
            }
            return 0;
        }

        public int UpdateLink(LinkInfo link)
        {
            string cmdText = string.Format(@"update [{0}links] set
                                [type]=@type,
                                [linkname]=@linkname,
                                [linkurl]=@linkurl,
                                [position]=@position,
                                [target]=@target,
                                [description]=@description,
                                [sortnum]=@sortnum,
                                [status]=@status,
                                [createtime]=@createtime
                                where linkid=@linkid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                OleDbHelper.MakeInParam("@type",OleDbType.Integer,4,link.Type),
								OleDbHelper.MakeInParam("@linkname",OleDbType.VarWChar,100,link.LinkName),
                                OleDbHelper.MakeInParam("@linkurl",OleDbType.VarWChar,255,link.LinkUrl),
                                OleDbHelper.MakeInParam("@position",OleDbType.Integer,4,link.Position),
                                OleDbHelper.MakeInParam("@target",OleDbType.VarWChar,50,link.Target),
								OleDbHelper.MakeInParam("@description",OleDbType.VarWChar,255,link.Description),
                                OleDbHelper.MakeInParam("@sortnum",OleDbType.Integer,4,link.SortNum),
								OleDbHelper.MakeInParam("@status",OleDbType.Integer,4,link.Status),
								OleDbHelper.MakeInParam("@createtime",OleDbType.Date,8,link.CreateTime),
                                OleDbHelper.MakeInParam("@linkid",OleDbType.Integer,4,link.LinkId),
							};

            return Convert.ToInt32(OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams));
        }

        public int DeleteLink(int linkId)
        {
            string cmdText = string.Format("delete from [{0}links] where [linkid] = @linkid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@linkid",OleDbType.Integer,4,linkId)
							};
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);


        }


        public List<LinkInfo> GetLinkList()
        {

            string cmdText = string.Format("select * from [{0}links]  order by [sortnum] asc,[linkid] asc",ConfigHelper.Tableprefix);

            return DataReaderToListLink(OleDbHelper.ExecuteReader(cmdText));

        }

        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param LinkName="read">OleDbDataReader</param>
        /// <param name="read"></param>
        /// <returns>LinkInfo</returns>
        private static List<LinkInfo> DataReaderToListLink(OleDbDataReader read)
        {
            var list = new List<LinkInfo>();
            while (read.Read())
            {
                var link = new LinkInfo
                               {
                                   LinkId = Convert.ToInt32(read["Linkid"]),
                                   Type = Convert.ToInt32(read["Type"]),
                                   LinkName = Convert.ToString(read["LinkName"]),
                                   LinkUrl = Convert.ToString(read["LinkUrl"]),
                                   Target = Convert.ToString(read["Target"]),
                                   Description = Convert.ToString(read["Description"]),
                                   SortNum = Convert.ToInt32(read["SortNum"]),
                                   Status = Convert.ToInt32(read["Status"]),
                                   CreateTime = Convert.ToDateTime(read["CreateTime"])
                               };
                if (read["Position"] != DBNull.Value)
                {
                    link.Position = Convert.ToInt32(read["Position"]);
                }



                list.Add(link);
            }
            read.Close();
            return list;
        }

    }
}
