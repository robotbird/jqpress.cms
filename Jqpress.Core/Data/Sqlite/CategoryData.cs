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
        private static void CheckSlug(CategoryInfo cate)
        {
            while (true)
            {
                string cmdText = cate.CategoryId == 0 ? string.Format("select count(1) from [{2}category] where [Slug]='{0}' and [type]={1}", cate.Slug, (int)CategoryType.Category,ConfigHelper.Tableprefix) : string.Format("select count(1) from [{3}category] where [Slug]='{0}'  and [type]={1} and [categoryid]<>{2}", cate.Slug, (int)CategoryType.Category, cate.CategoryId,ConfigHelper.Tableprefix);
                int r = Convert.ToInt32(OleDbHelper.ExecuteScalar(cmdText));
                if (r == 0)
                {
                    return;
                }
                cate.Slug += "-2";
            }
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int InsertCategory(CategoryInfo category)
        {
            CheckSlug(category);

            string cmdText = string.Format(@"insert into [{0}category]
                            ([Type],[ParentId],[CateName],[Slug],[Description],[SortNum],[PostCount],[CreateTime])
                            values
                            ( @Type,@ParentId,@CateName,@Slug,@Description,@SortNum,@PostCount,@CreateTime)", ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                OleDbHelper.MakeInParam("@Type",OleDbType.Integer,1,(int)CategoryType.Category),
                                OleDbHelper.MakeInParam("@ParentId",OleDbType.Integer,4,category.ParentId),
								OleDbHelper.MakeInParam("@CateName",OleDbType.VarWChar,255,category.CateName),
                                OleDbHelper.MakeInParam("@Slug",OleDbType.VarWChar,255,category.Slug),
								OleDbHelper.MakeInParam("@Description",OleDbType.VarWChar,255,category.Description),
                                OleDbHelper.MakeInParam("@SortNum",OleDbType.Integer,4,category.SortNum),
								OleDbHelper.MakeInParam("@PostCount",OleDbType.Integer,4,category.PostCount),
								OleDbHelper.MakeInParam("@CreateTime",OleDbType.Date,8,category.CreateTime)
							};
            OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams);

            int newId = Convert.ToInt32(OleDbHelper.ExecuteScalar(string.Format("select top 1 [categoryid] from [{0}category] order by [categoryid] desc",ConfigHelper.Tableprefix)));

            return newId;
        }

        public int UpdateCategory(CategoryInfo category)
        {
            CheckSlug(category);

            string cmdText = string.Format(@"update [{0}category] set
                                [Type]=@Type,
                                [ParentId]=@ParentId,
                                [CateName]=@CateName,
                                [Slug]=@Slug,
                                [Description]=@Description,
                                [SortNum]=@SortNum,
                                [PostCount]=@PostCount,
                                [CreateTime]=@CreateTime
                                where categoryid=@categoryid", ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
                                OleDbHelper.MakeInParam("@Type",OleDbType.Integer,1,(int)CategoryType.Category),
                                OleDbHelper.MakeInParam("@ParentId",OleDbType.Integer,4,category.ParentId),
								OleDbHelper.MakeInParam("@CateName",OleDbType.VarWChar,255,category.CateName),
                                OleDbHelper.MakeInParam("@Slug",OleDbType.VarWChar,255,category.Slug),
								OleDbHelper.MakeInParam("@Description",OleDbType.VarWChar,255,category.Description),
                                OleDbHelper.MakeInParam("@SortNum",OleDbType.Integer,4,category.SortNum),
								OleDbHelper.MakeInParam("@PostCount",OleDbType.Integer,4,category.PostCount),
								OleDbHelper.MakeInParam("@CreateTime",OleDbType.Date,8,category.CreateTime),
                                OleDbHelper.MakeInParam("@categoryid",OleDbType.Integer,1,category.CategoryId),
							};
            return Convert.ToInt32(OleDbHelper.ExecuteScalar(CommandType.Text, cmdText, prams));
        }

        public int DeleteCategory(int categoryId)
        {
            string cmdText = string.Format("delete from [{0}category] where [categoryid] = @categoryid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@categoryid",OleDbType.Integer,4,categoryId)
							};
            return OleDbHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);


        }

        public CategoryInfo GetCategory(int categoryId)
        {
            string cmdText = string.Format("select * from [{0}category] where [categoryid] = @categoryid",ConfigHelper.Tableprefix);
            OleDbParameter[] prams = { 
								OleDbHelper.MakeInParam("@categoryid",OleDbType.Integer,4,categoryId)
							};

            List<CategoryInfo> list = DataReaderToListCate(OleDbHelper.ExecuteReader(CommandType.Text, cmdText, prams));
            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// 获取全部分类
        /// </summary>
        /// <returns></returns>
        public List<CategoryInfo> GetCategoryList()
        {
            string condition = " [type]=" + (int)CategoryType.Category;

            string cmdText = string.Format("select * from [{0}category] where " + condition + "  order by [SortNum] asc,[categoryid] asc",ConfigHelper.Tableprefix);

            return DataReaderToListCate(OleDbHelper.ExecuteReader(cmdText));

        }

        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param name="read">OleDbDataReader</param>
        /// <returns>CategoryInfo</returns>
        private static List<CategoryInfo> DataReaderToListCate(OleDbDataReader read)
        {
            var list = new List<CategoryInfo>();
            while (read.Read())
            {
                var category = new CategoryInfo
                                   {
                                       CategoryId = Convert.ToInt32(read["categoryid"]),
                                       ParentId = Jqpress.Framework.Utils.TypeConverter.ObjectToInt(read["ParentId"],0),
                                       CateName = Convert.ToString(read["CateName"]),
                                       Slug = Convert.ToString(read["Slug"]),
                                       Description = Convert.ToString(read["Description"]),
                                       SortNum = Convert.ToInt32(read["SortNum"]),
                                       PostCount = Convert.ToInt32(read["PostCount"]),
                                       CreateTime = Convert.ToDateTime(read["CreateTime"])
                                   };
                //  category.Type = Convert.ToInt32(read["Type"]);

                list.Add(category);
            }
            read.Close();
            return list;
        }
    }
}
