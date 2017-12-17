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
    public class TagController : Controller
    {
        #region private items
        private TagService _tagService = new TagService();
        #endregion;
        public ActionResult List()
        {
            var model = new TagListModel();
            const int pageSize = 10;
            int count = 0;
            int pageIndex = PressRequest.GetInt("page", 1);
            var list = _tagService.GetTagListPage(pageSize, pageIndex, out count);
            model.PageList.LoadPagedList(list);
            model.TagList = (List<TagInfo>)list;
            return View(model);
        }

        /// <summary>
        /// get category by id
        /// </summary>
        public JsonResult Edit(int? id)
        {
            var cid = id ?? 0;
            TagInfo cat = _tagService.GetTag(cid);
            return Json(cat, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// delete categroy by id
        /// </summary>
        public ContentResult Delete(int? id)
        {
            var cid = id ?? 0;
            _tagService.DeleteTag(cid);
            return Content("success");
        }
        /// <summary>
        /// insert or update category
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Save"), ValidateInput(false)]
        public JsonResult Save(TagInfo cat)
        {

            if (string.IsNullOrEmpty(cat.PageName))
            {
                cat.PageName = cat.CateName;
            }

            cat.PageName = HttpHelper.HtmlEncode(StringHelper.FilterPageName(cat.PageName, "tag"));
            cat.SortNum = TypeConverter.StrToInt(cat.SortNum, 1000);
            if (cat.TagId > 0)
            {
                var oldcat = _tagService.GetTag(cat.TagId);
                cat.CreateTime = oldcat.CreateTime;
                cat.PostCount = oldcat.PostCount;
                _tagService.UpdateTag(cat);
            }
            else
            {
                cat.CreateTime = DateTime.Now;
                cat.PostCount = 0;
                _tagService.InsertTag(cat);
            }
            return Json(cat, JsonRequestBehavior.AllowGet);
        }

    }
}
