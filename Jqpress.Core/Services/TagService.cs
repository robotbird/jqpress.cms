using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Core.Repositories.Repository;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Services
{
   public class TagService
    {
        private ITagRepository _tagRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public TagService()
            : this(new TagRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="tagRepository"></param>
        public TagService(ITagRepository tagRepository)
        {
            this._tagRepository = tagRepository;
        }
        #endregion

        /// <summary>
        /// 标签列表
        /// </summary>
        private  List<TagInfo> Tags
        {
            get
            {
                return _tagRepository.Table.ToList();
            }
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        public  List<TagInfo> GetTagList()
        {
            return Tags;
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public  int InsertTag(TagInfo tag)
        {
            tag.TagId = _tagRepository.Insert(tag);
            return tag.TagId;
        }

        /// <summary>
        /// 修改标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public  int UpdateTag(TagInfo tag)
        {
            return _tagRepository.Update(tag);
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public  int DeleteTag(int tagId)
        {
            return _tagRepository.Delete(new TagInfo() { TagId= tagId});
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public  TagInfo GetTag(int tagId)
        {
            return _tagRepository.GetById(tagId);
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public  TagInfo GetTag(string name)
        {
           return _tagRepository.GetTagByName(name);   
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        public  TagInfo GetTagByPageName(string pagename)
        {
            return _tagRepository.GetTagByPageName(pagename);
        }


        /// <summary>
        /// 获取指定条数标签
        /// </summary>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public  List<TagInfo> GetTagList(int rowCount)
        {
            if (Tags.Count <= rowCount)
            {
                return Tags;
            }
            else
            {
                var list = Tags.AsQueryable().OrderByDescending(t => t.PostCount).ToList();
                list = list.Take(rowCount).ToList();
                return list;
            }
        }

        /// <summary>
        /// 获取分页标签
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public  List<TagInfo> GetTagList(int pageSize, int pageIndex, out int recordCount)
        {
            recordCount = Tags.Count;
            List<TagInfo> rlist = new List<TagInfo>();

            int start = (pageIndex - 1) * pageSize;
            int end = start + pageSize;
            if (end > Tags.Count)
            {
                end = Tags.Count;
            }
            for (int i = start; i < end; i++)
            {
                rlist.Add(Tags[i]);
            }
            return rlist;
        }

        /// <summary>
        /// 获取分页标签
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public  IPagedList<TagInfo> GetTagListPage(int pageSize, int pageIndex, out int recordCount)
        {
            recordCount = Tags.Count;
            List<TagInfo> list = new List<TagInfo>();

            int start = (pageIndex - 1) * pageSize;
            int end = start + pageSize;
            if (end > Tags.Count)
            {
                end = Tags.Count;
            }
            for (int i = start; i < end; i++)
            {
                list.Add(Tags[i]);
            }
            return new PagedList<TagInfo>(list, pageIndex - 1, pageSize, recordCount);

        }


        /// <summary>
        /// 获取ID
        /// </summary>
        /// <param name="tagID"></param>
        /// <returns></returns>
        public  int GetTagId(string pagename)
        {

            foreach (TagInfo t in Tags)
            {

                if (!string.IsNullOrEmpty(pagename) && t.PageName.ToLower() == pagename.ToLower())
                {
                    return t.TagId;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取标签名称
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public  string GetTagName(int tagId)
        {

            foreach (TagInfo t in Tags)
            {
                if (t.TagId == tagId)
                {
                    return t.CateName;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据标签Id获取标签列表
        /// </summary>
        /// <param name="ids">标签Id,逗号隔开</param>
        /// <returns></returns>
        public  List<TagInfo> GetTagList(string ids)
        {


            List<TagInfo> list = GetTagList();

            List<TagInfo> list2 = new List<TagInfo>();

            string[] tempids = ids.Split(',');

            //foreach (TagInfo term in list)
            //{
            //    foreach (string str in tempids)
            //    {
            //        if (term.TagId.ToString() == str)
            //        {
            //            list2.Add(term);

            //            continue;
            //        }
            //    }
            //}
            foreach (string str in tempids)
            {
                foreach (TagInfo tag in list)
                {
                    if (tag.TagId.ToString() == str)
                    {
                        list2.Add(tag);
                        continue;
                    }
                }
            }
            return list2;
        }
        /// <summary>
        /// 由标签名称列表返回标签ID列表,带{},新标签自动添加
        /// </summary>
        /// <param name="tagNameList"></param>
        /// <returns></returns>
        public  string GetTagIdList(string tagNames)
        {
            if (string.IsNullOrEmpty(tagNames))
            {
                return string.Empty;
            }
            string tagIds = string.Empty;
            tagNames = tagNames.Replace("，", ",");

            string[] names = tagNames.Split(',');

            foreach (string n in names)
            {
                if (!string.IsNullOrEmpty(n))
                {
                    TagInfo t = GetTag(n);

                    if (t == null)
                    {
                        t = new TagInfo();

                        t.PostCount = 0;
                        t.CreateTime = DateTime.Now;
                        t.Description = n;
                        t.SortNum = 1000;
                        t.CateName = n;
                        t.PageName = HttpHelper.HtmlEncode(StringHelper.FilterPageName(n, "tag"));

                        t.TagId = InsertTag(t);
                    }
                    tagIds += "{" + t.TagId + "}";
                }
            }
            return tagIds;
        }
        /// <summary>
        /// 更新标签对应文章数
        /// </summary>
        /// <param name="tagids">格式:{2}{26}</param>
        /// <returns></returns>
        public  bool UpdateTagUseCount(string tagids, int addCount)
        {
            if (string.IsNullOrEmpty(tagids))
            {
                return false;
            }

            string[] tagidlist = tagids.Replace("{", "").Split('}');

            foreach (string tagId in tagidlist)
            {
                TagInfo tag = GetTag(Jqpress.Framework.Utils.TypeConverter.StrToInt(tagId, 0));
                if (tag != null)
                {
                    tag.PostCount += addCount;
                    _tagRepository.Update(tag);
                }
            }
            return true;
        }
    }
}
