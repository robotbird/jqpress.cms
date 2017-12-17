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
    public partial class TagRepository:ITagRepository
    {
        
        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        private  void CheckPageName(TagInfo cate)
        {
            while (true)
            {
                string cmdText = cate.TagId == 0 ? string.Format("select count(1) from [{2}category] where [PageName]='{0}' and [type]={1}", cate.PageName, (int)CategoryType.Tag, ConfigHelper.Tableprefix) : string.Format("select count(1) from [{3}category] where [PageName]='{0}'  and [type]={1} and [categoryid]<>{2}", cate.PageName, (int)CategoryType.Tag, cate.TagId, ConfigHelper.Tableprefix);
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

        public int Insert(TagInfo tag)
        {
            CheckPageName(tag);

            string cmdText = string.Format(@"insert into [{0}category]
                            (
                            [Type],[ParentId],[CateName],[PageName],[Description],[SortNum],[PostCount],[CreateTime]
                            )
                            values
                            (
                            @Type,@ParentId,@CateName,@PageName,@Description,@SortNum,@PostCount,@CreateTime
                            )", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                conn.Execute(cmdText, tag);
                //return conn.Query<int>(string.Format("select top 1 [categoryid] from [{0}category] order by [categoryid] desc", ConfigHelper.Tableprefix), null).First();
                return conn.Query<int>(string.Format("select  [categoryid] from [{0}category] order by [categoryid] desc limit 1", ConfigHelper.Tableprefix), null).First();//for sqlite
            }
        }

        public int Update(TagInfo tag)
        {
            CheckPageName(tag);

            string cmdText = string.Format(@"update [{0}category] set
                                [Type]=@Type,
                                [CateName]=@CateName,
                                [PageName]=@PageName,
                                [Description]=@Description,
                                [SortNum]=@SortNum,
                                [PostCount]=@PostCount,
                                [CreateTime]=@CreateTime
                                where categoryid=@categoryid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new
                {
                    Type = (int)CategoryType.Tag,
                    CateName = tag.CateName,
                    PageName = tag.PageName,
                    Description = tag.Description,
                    SortNum = tag.SortNum,
                    PostCount = tag.PostCount,
                    CreateTime = tag.CreateTime.ToString(),
                    categoryid = tag.TagId
                });
            }

        }

        public int Delete(TagInfo tag)
        {
            string cmdText = string.Format("delete from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new { categoryid = tag.TagId });
            }
        }

        public TagInfo GetById(object id)
        {
            string cmdText = string.Format("select * from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<TagInfo>(cmdText, new { categoryid = (int)id });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }
        public TagInfo GetTagByName(string name)
        {
            string cmdText = string.Format("select * from [{0}category] where [CateName] = @CateName", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<TagInfo>(cmdText, new { CateName = name });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }

        public TagInfo GetTagByPageName(string pagename)
        {
            string cmdText = string.Format("select * from [{0}category] where [PageName] = @PageName", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<TagInfo>(cmdText, new { PageName = pagename });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }

        /// <summary>
        /// 获取全部标签
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TagInfo> Table
        {
            get
            {
                string condition = " [type]=" + (int)CategoryType.Tag;

                string cmdText = string.Format("select * from [{0}category] where " + condition + " order by [SortNum] asc,[categoryid] asc", ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<TagInfo>(cmdText, null);
                    return list;
                }
            }

        }
        /// <summary>
        /// 根据多个标签id获取标签
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<TagInfo> GetTagList(int[] ids)
        {
            var result = from t in Table
                         where ids.Contains(t.TagId)
                         select t;
            return result.ToList();
        }


    }
}
