using System;
using System.Collections.Generic;
using Jqpress.Blog.Entity;
using System.Data.OleDb;
using System.Data;
using Jqpress.Framework.DbProvider.Access;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Entity.Enum;

namespace Jqpress.Blog.Data.Sqlite
{
    public partial class DataProvider
    {
        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        private static void CheckSlug(TagInfo cate)
        {
            while (true)
            {
                string cmdText = cate.TagId == 0 ? string.Format("select count(1) from [{2}category] where [Slug]='{0}' and [type]={1}", cate.Slug, (int)CategoryType.Tag,ConfigHelper.Tableprefix) : string.Format("select count(1) from [{3}category] where [Slug]='{0}'  and [type]={1} and [categoryid]<>{2}", cate.Slug, (int)CategoryType.Tag, cate.TagId, ConfigHelper.Tableprefix);
                int r = Convert.ToInt32(OleDbHelper.ExecuteScalar(cmdText));
                if (r == 0)
                {
                    return;
                }
                cate.Slug += "-2";
            }
        }

        public int InsertTag(TagInfo tag)
        {
            CheckSlug(tag);

            string cmdText =string.Format(@"insert into [{0}category]
                            (
                            [Type],[ParentId],[CateName],[Slug],[Description],[SortNum],[PostCount],[CreateTime]
                            )
                            values
                            (
                            @Type,@ParentId,@CateName,@Slug,@Description,@SortNum,@PostCount,@CreateTime
                            )", ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                OleDbHelper.MakeInParam("@Type",OleDbType.Integer,1,(int)CategoryType.Tag),
                                OleDbHelper.MakeInParam("@ParentId",OleDbType.Integer,4,0),
								OleDbHelper.MakeInParam("@CateName",OleDbType.VarWChar,255,tag.CateName),
                                OleDbHelper.MakeInParam("@Slug",OleDbType.VarWChar,255,tag.Slug),
								OleDbHelper.MakeInParam("@Description",OleDbType.VarWChar,255,tag.Description),
                                OleDbHelper.MakeInParam("@SortNum",OleDbType.Integer,4,tag.SortNum),
								OleDbHelper.MakeInParam("@PostCount",OleDbType.Integer,4,tag.PostCount),
								OleDbHelper.MakeInParam("@CreateTime",OleDbType.Date,8,tag.CreateTime)
							};
            OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams);

            int newId = Convert.ToInt32(OleDbHelper.ExecuteScalar(string.Format("select top 1 [categoryid] from [{0}category] order by [categoryid] desc",ConfigHelper.Tableprefix)));

            return newId;
        }

        public int UpdateTag(TagInfo tag)
        {
            CheckSlug(tag);

            string cmdText = string.Format(@"update [{0}category] set
                                [Type]=@Type,
                                [CateName]=@CateName,
                                [Slug]=@Slug,
                                [Description]=@Description,
                                [SortNum]=@SortNum,
                                [PostCount]=@PostCount,
                                [CreateTime]=@CreateTime
                                where categoryid=@categoryid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                OleDbHelper.MakeInParam("@Type",OleDbType.Integer,1,(int)CategoryType.Tag),
								OleDbHelper.MakeInParam("@CateName",OleDbType.VarWChar,255,tag.CateName),
                                OleDbHelper.MakeInParam("@Slug",OleDbType.VarWChar,255,tag.Slug),
								OleDbHelper.MakeInParam("@Description",OleDbType.VarWChar,255,tag.Description),
                                OleDbHelper.MakeInParam("@SortNum",OleDbType.Integer,4,tag.SortNum),
								OleDbHelper.MakeInParam("@PostCount",OleDbType.Integer,4,tag.PostCount),
								OleDbHelper.MakeInParam("@CreateTime",OleDbType.Date,8,tag.CreateTime),
                                OleDbHelper.MakeInParam("@categoryid",OleDbType.Integer,1,tag.TagId),
							};
            return Convert.ToInt32(OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams));
        }

        public int DeleteTag(int tagId)
        {
            string cmdText = string.Format("delete from [{0}category] where [categoryid] = @categoryid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@categoryid",OleDbType.Integer,4,tagId)
							};
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);


        }

        public TagInfo GetTag(int tagId)
        {
            string cmdText = string.Format("select * from [{0}category] where [categoryid] = @categoryid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@categoryid",OleDbType.Integer,4,tagId)
							};

            List<TagInfo> list = DataReaderToListTag(OleDbHelper.ExecuteReader(CommandType.Text, cmdText, prams));
            return list.Count > 0 ? list[0] : null;
        }


        public List<TagInfo> GetTagList()
        {
            string condition = " [type]=" + (int)CategoryType.Tag;

            string cmdText = string.Format("select * from [{0}category] where " + condition + "  order by [SortNum] asc ,[categoryid] asc",ConfigHelper.Tableprefix);

            return DataReaderToListTag(OleDbHelper.ExecuteReader(cmdText));

        }

        public List<TagInfo> GetTagList(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return new List<TagInfo>();
            }

            string cmdText = string.Format("select * from [{0}category] where  [categoryid] in (" + ids + ")",ConfigHelper.Tableprefix);

            //  throw new Exception(cmdText);

            return DataReaderToListTag(OleDbHelper.ExecuteReader(cmdText));
        }

        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param CateName="read">OleDbDataReader</param>
        /// <param name="read"></param>
        /// <returns>TagInfo</returns>
        private static List<TagInfo> DataReaderToListTag(OleDbDataReader read)
        {
            var list = new List<TagInfo>();
            while (read.Read())
            {
                var tag = new TagInfo
                              {
                                  TagId = Convert.ToInt32(read["categoryid"]),
                                  CateName = Convert.ToString(read["CateName"]),
                                  Slug = Convert.ToString(read["Slug"]),
                                  Description = Convert.ToString(read["Description"]),
                                  SortNum = Convert.ToInt32(read["SortNum"]),
                                  PostCount = Convert.ToInt32(read["PostCount"]),
                                  CreateTime = Convert.ToDateTime(read["CreateTime"])
                              };
                //  tag.Type = Convert.ToInt32(read["Type"]);

                list.Add(tag);
            }
            read.Close();
            return list;
        }

    }
}
