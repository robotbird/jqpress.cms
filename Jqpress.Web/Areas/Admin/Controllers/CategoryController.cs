using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Web;
using Jqpress.Framework.Utils;
using Jqpress.Core.Services;
using Jqpress.Core.Domain;
using Jqpress.Web.Areas.Admin.Models;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class CategoryController : BaseAdminController
    {
        #region private items
        private CategoryService _categoryService = new CategoryService();
        #endregion;
        public ActionResult List() 
        {
            var model = new CateListModel();

            int cateid = PressRequest.GetQueryInt("cateid", -1);
            var catelist = _categoryService.GetCategoryTreeList();
            catelist.Remove(catelist.Find(c=>c.CategoryId==0));
            model.CateList = catelist;
            model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.TreeChar+c.CateName, Value = c.CategoryId.ToString(), Selected = c.CategoryId == cateid });

            return View(model);
        }

        /// <summary>
        /// get category by id
        /// </summary>
        public ActionResult Edit(int? id)
        {
            var cid = id??0;

            var model = new CateModel();
            if (cid > 0) 
            {
                model.Category = _categoryService.GetCategory(cid);            
            }
            var catelist = _categoryService.GetCategoryTreeList();
            model.CateList = catelist;
            catelist.Add(new CategoryInfo() { CateName = "作为一级分类", CategoryId = 0 });

            model.CateType.Clear();
            model.CateType.Add(new SelectListItem { Text = "产品列表页", Value = "1", Selected = model.Category.Type == (int)Jqpress.Core.Domain.Enum.CateType.ProList});
            model.CateType.Add(new SelectListItem { Text = "信息列表页", Value = "2", Selected = model.Category.Type == (int)Jqpress.Core.Domain.Enum.CateType.ItemList});
            model.CateType.Add(new SelectListItem { Text = "单页面", Value = "3", Selected = model.Category.Type == (int)Jqpress.Core.Domain.Enum.CateType.Page});


            
            model.CateSelectItem.Clear();
            model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.TreeChar + c.CateName, Value = c.CategoryId.ToString(), Selected = c.CategoryId == model.Category.ParentId });
            
            return View(model);
        }
        /// <summary>
        /// delete categroy by id
        /// </summary>
        public ActionResult Delete(int? id)
        {
            var cid = id ?? 0;
            _categoryService.DeleteCategory(cid);
            SuccessNotification("删除成功");
            return RedirectToAction("list");
        }
        /// <summary>
        /// insert or update category
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Save"), ValidateInput(false)]        
        public ActionResult Save(CategoryInfo cat)
        {

            if (string.IsNullOrEmpty(cat.PageName))
            {
                cat.PageName = cat.CateName;
            }

            cat.PageName = HttpHelper.HtmlEncode(StringHelper.FilterPageName(cat.PageName, "cate"));
            cat.SortNum = TypeConverter.StrToInt(cat.SortNum, 1000);
            if (cat.CategoryId > 0)
            {
                var  oldcat = _categoryService.GetCategory(cat.CategoryId);
                cat.CreateTime = oldcat.CreateTime;
                cat.PostCount = oldcat.PostCount;
                _categoryService.UpdateCategory(cat);
                SuccessNotification("修改成功");
            }
            else 
            {
                cat.CreateTime = DateTime.Now;
                cat.PostCount = 0;
                cat.CategoryId = _categoryService.InsertCategory(cat);
                SuccessNotification("添加成功");

            }
            cat.TreeChar = _categoryService.GetCategoryTreeList().Find(c => c.CategoryId == cat.CategoryId).TreeChar;

            return Redirect("edit?id=" + cat.CategoryId );

        }

    }
}
