using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Exceptions;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CoreCms.Ef;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminHomeController : AdminBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HomePageSettings()
        {
            var model = new HomePageSettingsAdminPage();
            model.Categories=new List<FeCategory>();

            List<Guid> cats;
            List<ContentLanguage> contentLangs;
            using (var db = new CoreCmsDbContext())
            {
                 cats = db.Categories.Where(i=>i.Deleted==false).Select(i => i.Id).ToList();

                contentLangs = db.ContentLanguages.Where(i => cats.Contains(i.Id)).ToList();
            }
            foreach (var c in cats)
            {
                model.Categories.Add(new FeCategory()
                {
                    Id=c,
                    Title= contentLangs.GetValue(c,"Title")
                });
                
            }
            var rootPath = Server.MapPath("~/");
            var pdir = Server.MapPath("~/Views/HomePageSection/");
            if (Directory.Exists(pdir))
            {
                var pfiles = Directory.GetFiles(pdir, "*.cshtml");
                model.ListViewName = pfiles.Select(i => i.Replace(rootPath, "~/").Replace("\\", "/")).ToList();
            }
            else
            {
                model.ListViewName = new List<string>();
            }

            return View(model);
        }

        public JsonResult CategoryTree()
        {
            var data = new FeTreeNodeBuilder().GetAllCategoryForSideMenu(null,LanguageId, null, false);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListHomePageSection()
        {

            List<ContentLanguage> contentLangs;
            List<HomePageSettingsAdminPage.Section> sections;
            using (var db = new CoreCmsDbContext())
            {
                sections = db.HomePageSections.OrderBy(i => i.DisplayOrder)
                .Select(i => new HomePageSettingsAdminPage.Section()
                {
                    Id = i.Id,
                    Published = i.Published,
                    CategoryId = i.CategoryId,
                    DisplayOrder = i.DisplayOrder,
                    HomePageSectionViewName = i.HomePageSectionViewName,

                }).ToList();

                var ids = sections.Select(i => i.Id).ToList();
                ids.AddRange(sections.Select(i=>i.CategoryId).ToList());

                contentLangs = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }

            foreach (var ms in sections)
            {
                ms.Title = contentLangs.GetValue(ms.Id, "Title");
                ms.CategoryTitle = contentLangs.GetValue(ms.CategoryId, "Title");
            }

            return Json(new { sections.Count, rows = sections, success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HomePageSectionDetail(Guid id)
        {
            HomePageSettingsAdminPage.Section section = new HomePageSettingsAdminPage.Section();
            using (var db = new CoreCmsDbContext())
            {
                section = db.HomePageSections.Where(i => i.Id == id).Select(i => new HomePageSettingsAdminPage.Section()
                {
                    Id = i.Id,
                    Published = i.Published,
                    CategoryId = i.CategoryId,
                    DisplayOrder = i.DisplayOrder,
                    HomePageSectionViewName = i.HomePageSectionViewName,

                }).FirstOrDefault();

                var contentLangs = db.ContentLanguages.Where(i => i.Id == id || i.Id== section.CategoryId).ToList();

                section.Title = contentLangs.GetValue(id, "Title");
                section.CategoryTitle = contentLangs.GetValue(section.CategoryId, "Title");
            }

            return Json(new { Ok = true, Data = section, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CreateHomeSection(string title, Guid categoryId, string viewName, short displayOrder)
        {
            var id = Guid.NewGuid();

            MemoryMessageBuss.PushCommand(new CreateHomePageSection(id, title, categoryId, LanguageId, displayOrder, viewName, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateHomeSection(Guid id, string title, Guid categoryId, string viewName, short displayOrder)
        {

            MemoryMessageBuss.PushCommand(new UpdateHomePageSection(id, title, categoryId, LanguageId, displayOrder, viewName, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PublishHomeSection(Guid id, bool isPublish)
        {

            MemoryMessageBuss.PushCommand(new PublishHomePageSection(id, isPublish, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteHomeSection(Guid id, string title, Guid categoryId, string viewName, short displayOrder,
            bool publish)
        {

            MemoryMessageBuss.PushCommand(new DeleteHomPageSection(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [AdminLoginRequired(true)]
        [AllowAnonymous]
        public ActionResult Login(string error, string url)
        {
            return View();
        }

        [HttpPost]
        [AdminLoginRequired(true)]
        [AllowAnonymous]
        public ActionResult Login(FormCollection formCollection, string username, string password, string url = "")
        {
            User u = null;
            try
            {
                u = UserSessionContext.Dologin(username, password);
            }
            catch (Exception ex)
            {
                return Redirect($"/Admin/AdminHome/Login/?error={HttpUtility.UrlEncode(ex.ToMessage())}");
            }

            if (u == null)
            {
                return View();
            }

            if (UserSessionContext.CurrentUserIsSysAdmin(u.TokenSession) || string.IsNullOrEmpty(url))
            {
                return Redirect("~/Admin");
            }

            url = HttpUtility.UrlDecode(url);
            if (url.Equals(UserSessionContext.UrlAdminLogin, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect("~/");
            }

            return Redirect(url);

        }

        [AdminLoginRequired(true)]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            try
            {
                UserSessionContext.Dologout();
            }
            catch { }

            return Redirect(UserSessionContext.UrlAdminLogin);
        }
    }
}