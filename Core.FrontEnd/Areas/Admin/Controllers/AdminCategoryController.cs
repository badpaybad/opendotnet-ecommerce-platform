using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.CoreCms.Commands;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Utils;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminCategoryController : AdminBaseController
    {
        public ActionResult Index()
        {
            var model = new CategoryAdminPage();

            model.ListCategoryType = EnumsExtensions.ListCategoryType();
            var rootPath = Server.MapPath("~/");

            var ndir = Server.MapPath("~/Views/Category/");
            if (Directory.Exists(ndir))
            {
                var files = Directory.GetFiles(ndir, "*.cshtml");
                model.ListCategoryViewName = files.Select(i => i.Replace(rootPath, "~/").Replace("\\", "/")).ToList();
            }
            else
            {
                model.ListCategoryViewName = new List<string>();
            }

            var pdir = Server.MapPath("~/Views/CategoryProduct/");
            if (Directory.Exists(pdir))
            {
                var pfiles = Directory.GetFiles(pdir, "*.cshtml");
                model.ListCategoryProductViewName = pfiles.Select(i => i.Replace(rootPath, "~/").Replace("\\", "/")).ToList();
            }
            else
            {
                model.ListCategoryProductViewName = new List<string>();
            }

            return View(model);
        }

        public JsonResult CategoryTree()
        {
            var data = new FeTreeNodeBuilder().GetAllCategoryForSideMenu(null,LanguageId,null, false);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeRoot(Guid id, Guid parentId, int displayOrder)
        {
            MemoryMessageBuss.PushCommand(new ChangeRootCategory(id, parentId, CurrentUserId, DateTime.Now));

            MemoryMessageBuss.PushCommand(new ChangeCategoryDisplayOrder(id, displayOrder, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Detail(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Json(new { Ok = true, Data = new { }, Message = "Input to add new" }, JsonRequestBehavior.AllowGet);
            }

            using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
            {
                var data = db.Categories.Where(i => i.Id == id)
                    .Select(c => new FeCategory()
                    {
                        Id = c.Id,
                        IsSinglePage = c.IsSinglePage,
                        ShowInFrontEnd = c.ShowInFrontEnd,
                        CategoryViewName = c.CategoryViewName,
                        Type= (Enums.CategoryType) c.Type
                    }).FirstOrDefault();

                data.Title = db.ContentLanguages.GetValue(id, LanguageId, "Title", "Category");
                data.SeoUrlFriendly = db.ContentLanguages.GetValue(id, LanguageId, "SeoUrlFriendly", "Category");
                data.SeoDescription = db.ContentLanguages.GetValue(id, LanguageId, "SeoDescription", "Category");
                data.SeoKeywords = db.ContentLanguages.GetValue(id, LanguageId, "SeoKeywords", "Category");

                return Json(new { Ok = true, Data = data, Message = "Input to edit" }, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult Save(Guid id, string title, string categoryViewName
            ,string seoKeywords,string seoDescription, string seoUrlFriendly
            , bool isSinglePage, bool showInFrontEnd, short categoryType, bool isSaveNew = false)
        {
            var xtype = (Enums.CategoryType)categoryType;

            if (id == Guid.Empty || isSaveNew)
            {
                id = Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateCategory(id, isSinglePage, showInFrontEnd, title
                    ,seoKeywords,seoDescription, seoUrlFriendly
                    , categoryViewName, string.Empty, title, LanguageId, Guid.Empty, xtype, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateCategory(id, isSinglePage, showInFrontEnd, title
                    , seoKeywords, seoDescription, seoUrlFriendly
                    , categoryViewName, string.Empty, title, LanguageId, xtype, CurrentUserId, DateTime.Now));
            }

            return Json(new { Ok = true, Data = new { Id = id, Title = title }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Delete(Guid id)
        {

            MemoryMessageBuss.PushCommand(new DeleteCategory(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
    }
}