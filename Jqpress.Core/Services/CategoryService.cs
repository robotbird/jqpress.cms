using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.Cache;
using Jqpress.Core.Domain;
using Jqpress.Core.Repositories.Repository;
using Jqpress.Core.Repositories.IRepository;


namespace Jqpress.Core.Services
{
   public class CategoryService
    {
        private ICategoryRepository _categoryRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public CategoryService()
            : this(new CategoryRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="categoryRepository"></param>
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }
        #endregion

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public  int InsertCategory(CategoryInfo category)
        {
            int categoryId = _categoryRepository.Insert(category);
            category.CategoryId = categoryId;
            CacheHelper.Remove("categoryTree");
            CacheHelper.Remove("category");
            return categoryId;
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public  int UpdateCategory(CategoryInfo category)
        {
            CacheHelper.Remove("categoryTree");
            CacheHelper.Remove("category");
            return _categoryRepository.Update(category);
        }

        /// <summary>
        /// 更新文章数
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public  int UpdateCategoryCount(int categoryId, int addCount)
        {
            if (categoryId == 0)
            {
                return 0;
            }

            CategoryInfo category = GetCategory(categoryId);
            if (category != null)
            {
                category.PostCount += addCount;

                return UpdateCategory(category);
            }
            return 0;
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public  int DeleteCategory(int categoryId)
        {
            CacheHelper.Remove("categoryTree");
            CacheHelper.Remove("category");
            return _categoryRepository.Delete(new CategoryInfo { CategoryId = categoryId });
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public  CategoryInfo GetCategory(int categoryId)
        {
            return _categoryRepository.GetById(categoryId);
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        public  CategoryInfo GetCategory(string pagename)
        {
            if (Jqpress.Framework.Utils.Validate.IsInt(pagename)) 
            {
                return GetCategory(Convert.ToInt32(pagename));
            }
            return _categoryRepository.GetCategoryByPageName(pagename);
        }

        /// <summary>
        /// 获取分类名称
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public  string GetCategoryName(int categoryId)
        {
            return GetCategory(categoryId).CateName;
        }

        /// <summary>
        /// 获取全部分类
        /// </summary>
        /// <returns></returns>
        public  List<CategoryInfo> GetCategoryList()
        {
            return _categoryRepository.Table.ToList();
        }

        /// <summary>
        /// 获取全部分类
        /// </summary>
        /// <returns></returns>
        public  List<CategoryInfo> GetCategoryTreeList()
        {
            string categoryTree = "categoryTree";

            //这里面从内存读数据，所以不会遍历查库，暂时查库，要不他会将遍历后的值传给_categories
            List<CategoryInfo> listTree = (List<CategoryInfo>)CacheHelper.Get(categoryTree);

            if (listTree == null)
            {
                listTree = GetCategoryTree(0, GetCategoryList(), 0);

                CacheHelper.Insert(categoryTree, listTree, CacheHelper.HourFactor * 12);
            }
            return listTree;
        }

        /// <summary>
        /// 第一次取出顶级父类，最后所有的泛型都放在list，直接输出list
        /// 一个分类一个分类的遍历，如果顶级分类下有子分类，则将子分类的树加一个└等级加入list，
        /// 并将子分类的cateid最为递归的prentid继续执行递归操作，直到所有分类下都无子分类
        /// </summary>
        /// <param name="parentid"></param>
        /// <param name="listcate"></param>
        /// <param name="level">递归等级</param>
        /// <returns></returns>
        private  List<CategoryInfo> GetCategoryTree(int parentid, List<CategoryInfo> listcate, int level)
        {
            string categoryCacheKey = "category";

            //这里面从内存读数据，所以不会遍历查库，暂时查库，要不他会将遍历后的值传给_categories
            List<CategoryInfo> listAll = (List<CategoryInfo>)CacheHelper.Get(categoryCacheKey);

            if (listAll == null)
            {
                listAll = _categoryRepository.Table.ToList();

                CacheHelper.Insert(categoryCacheKey, listAll, CacheHelper.HourFactor * 12);
            }

            //if (listAll.FindAll(c => !string.IsNullOrEmpty(c.TreeChar)).Count>0) {
            //    _categories = _categoryRepository.Table.ToList();
            //    listAll = _categories;
            //}
            var treelist = new List<CategoryInfo>();


            foreach (var cate in listcate)
            {
                if (cate.ParentId == parentid)
                {
                    cate.Depth = level;

                    if (level > 0)
                    {
                        for (int i = 0; i < level; i++)
                        {
                            cate.TreeChar += " └ ";//父类的TreeChar加当前的TreeChar
                        }

                    }
                    else
                    {
                        cate.Path = cate.CategoryId.ToString();//当前cateid
                        cate.TreeChar = "";

                    }
                    treelist.Add(cate);
                    //如果它还有子分类 则继续递归
                    var childlist = listAll.FindAll(c => c.ParentId == cate.CategoryId);
                    if (childlist.Count > 0)
                    {
                        foreach (var child in GetCategoryTree(cate.CategoryId, childlist, level + 1))
                        {
                            treelist.Add(child);
                        }
                    }
                }
            }
            return treelist;
        }
    }
}
