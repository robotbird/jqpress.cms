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
    public partial class CategoryRepository:ICategoryRepository
    {
        

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual int Insert(CategoryInfo category)
        {
            CheckPageName(category);

            string cmdText = string.Format(@"insert into [{0}category]
                            ([ParentId],[CateName],[PageName],[Description],[SortNum],[PostCount],[CreateTime],[Type])
                            values
                            (@ParentId,@CateName,@PageName,@Description,@SortNum,@PostCount,@CreateTime,@Type)", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection()) 
            {
                conn.Execute(cmdText, category);
                //return conn.Query<int>(string.Format("select top 1 [categoryid] from [{0}category] order by [categoryid] desc", ConfigHelper.Tableprefix), null).First();
                return conn.Query<int>(string.Format("select  [categoryid] from [{0}category] order by [categoryid] desc limit 1", ConfigHelper.Tableprefix), null).First();//for sqlite
            }
        }
        /// <summary>
        /// 更新分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual int Update(CategoryInfo category)
        {
            CheckPageName(category);

            string cmdText = string.Format(@"update [{0}category] set
                                [ParentId]=@ParentId,
                                [CateName]=@CateName,
                                [PageName]=@PageName,
                                [Description]=@Description,
                                [SortNum]=@SortNum,
                                [PostCount]=@PostCount,
                                [CreateTime]=@CreateTime,
                                [Type]=@Type
                                where categoryid=@categoryid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
               return conn.Execute(cmdText, category);
            }
        }
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int Delete(CategoryInfo category)
        {
            string cmdText = string.Format("delete from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new { categoryid = category.CategoryId });
            }
        }
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CategoryInfo GetById(object id)
        {
            if ((int)id <= 0) 
            {
                return null;
            }
            string cmdText = string.Format("select * from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<CategoryInfo>(cmdText, new { categoryid = (int)id });
                return list.ToList().Count > 0 ? list.First() : null;
            }
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        public CategoryInfo GetCategoryByPageName(string pagename) 
        {
            string cmdText = string.Format("select * from [{0}category] where [pagename] = @pagename", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<CategoryInfo>(cmdText, new { pagename = pagename });
                return list.ToList().Count > 0 ? list.First() : null;
            }
        }

        /// <summary>
        /// 获取全部分类
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CategoryInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}category]  order by [SortNum] asc,[categoryid] asc", ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<CategoryInfo>(cmdText, null);
                    return list;
                }
            }

        }

        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        private  void CheckPageName(CategoryInfo cate)
        {
            while (true)
            {
                string cmdText = cate.CategoryId == 0 ? string.Format("select count(1) from [{2}category] where [PageName]='{0}' and [type]={1}", cate.PageName, (int)CategoryType.Category, ConfigHelper.Tableprefix) : string.Format("select count(1) from [{3}category] where [PageName]='{0}'  and [type]={1} and [categoryid]<>{2}", cate.PageName, (int)CategoryType.Category, cate.CategoryId, ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    int r = conn.Query<int>(cmdText, null).First();

                    if (r == 0)
                    {
                        return;
                    }
                }
                cate.PageName += "-2";
            }
        }
    }
}
