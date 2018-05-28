using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CoreCms.Ef;
using DomainDrivenDesign.CoreCms.Services;
using Core.FrontEnd.Areas.Admin.Models;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.CorePermission;
using Microsoft.Ajax.Utilities;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminNewsController : AdminBaseController
    {
        public ActionResult Index(Guid id)
        {
            var model = new NewsListAdminPage();

            using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
            {
                model.CategoryId = id;
                model.CategoryTitle = db.ContentLanguages.GetValue(id, LanguageId, "Title", "Category");

            }

            return View(model);
        }

        public ActionResult Edit(Guid id, Guid categoryId)
        {
            var model = new NewsEditAdminPage();
            using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
            {
                model.CategoryId = categoryId;
                model.CategoryTitle = db.ContentLanguages.GetValue(categoryId, LanguageId, "Title");

                var n = db.News.SingleOrDefault(i => i.Id == id) ?? new News();

                model.Id = id;
                model.Title = db.ContentLanguages.GetValue(id, LanguageId, "Title");
                model.ShortDescription = db.ContentLanguages.GetValue(id, LanguageId, "ShortDescription");
                model.Description = db.ContentLanguages.GetValue(id, LanguageId, "Description");
                model.SeoUrlFriendly = db.ContentLanguages.GetValue(id, LanguageId, "SeoUrlFriendly");
                model.SeoKeywords = db.ContentLanguages.GetValue(id, LanguageId, "SeoKeywords");
                model.UrlImage = db.ContentLanguages.GetValue(id, LanguageId, "UrlImage");
                model.Published = n.Published;
                model.SeoDescription = db.ContentLanguages.GetValue(id, LanguageId, "SeoDescription");
                model.CreatedDate = n.CreatedDate;
                model.AllowComment = n.AllowComment;

                model.NewsCategoies = db.RelationShips.Where(i => i.ToId == id).Select(i => i.FromId).ToList();

            }
            return View(model);
        }

        [HttpPost]
        public JsonResult SaveCategories(Guid id, List<Guid> categoryIds)
        {
            categoryIds = categoryIds.Where(i => i != Guid.Empty).ToList();

            MemoryMessageBuss.PushCommand(new ChangeNewsToCategories(id, categoryIds, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateInput(false)]
        public JsonResult Save(Guid categoryId, Guid id, string title, string shortDescription, string description, string urlImage, bool allowComment, bool isSaveNew = false)
        {
            if (id == Guid.Empty || isSaveNew)
            {
                id = Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateNews(id, title, shortDescription, description, urlImage, allowComment, LanguageId, Guid.Empty, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateNews(id, title, shortDescription, description, urlImage, allowComment, LanguageId, CurrentUserId, DateTime.Now));
            }

            MemoryMessageBuss.PushCommand(new AddNewsToCategory(id, categoryId, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id, Title = title }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveSeo(Guid id, string seoKeywords, string seoDescription, string seoUrlFriendly)
        {
            MemoryMessageBuss.PushCommand(new UpdateNewsForSeo(id, seoKeywords, seoDescription, LanguageId, seoUrlFriendly, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteNews(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Publish(Guid id, bool isPublish)
        {
            MemoryMessageBuss.PushCommand(new PublishNews(id, isPublish, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddComent(Guid id, string comment, Guid? commentParentId)
        {
            var x = commentParentId ?? Guid.Empty;
            string authorName = UserSessionContext.CurrentUsername();
            MemoryMessageBuss.PushCommand(new AddCommentToNews(id, comment, authorName, UserSessionContext.CurrentUserId(), x));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ListComments(Guid? newsId, int? skip, int? take, string sortField, string orderBy)
        {
            var xtake = 10;
            var xskip = 0;
            long total = 0;


            if (skip != null)
            {
                xskip = skip.Value;
            }
            if (take != null)
            {
                xtake = take.Value;
            }
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(FeNews.CreatedDate);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            var rows = new List<Comment>();
            var guidEmpty = Guid.Empty;
            var xnewsId = newsId ?? guidEmpty;

            if (xnewsId == guidEmpty)
            {
                using (var db = new CoreDbContext())
                {
                    var queryable = db.Comments.Where(i => i.CommentParentId == guidEmpty);
                    total = queryable.LongCount();
                    rows = queryable
                        .OrderBy(i => i.CreatedDate)
                        .Skip(xskip).Take(xtake).ToList();
                }
            }
            else
            {
                using (var db = new CoreDbContext())
                {
                    var queryable = db.Comments.Where(i => i.Id == xnewsId && i.CommentParentId == guidEmpty);
                    total = queryable.LongCount();
                    rows = queryable
                        .OrderBy(i => i.CreatedDate)
                        .Skip(xskip).Take(xtake).ToList();
                }
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List(Guid categoryId, string keywords,
            int? skip, int? take, string sortField, string orderBy)
        {
            var xtake = 10;
            var xskip = 0;
            long total = 0;
            if (skip != null)
            {
                xskip = skip.Value;
            }
            if (take != null)
            {
                xtake = take.Value;
            }
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(FeNews.CreatedDate);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            List<FeNews> rows = new List<FeNews>();
            List<ContentLanguage> contentLanguages = null;

            var tempNews = NewsSearchServices.Search(keywords, LanguageId, new List<Guid>() { categoryId }, null, xskip, xtake,
                  out contentLanguages, out total);

            foreach (var n in tempNews)
            {
                var i = new FeNews();
                i.CreatedDate = n.CreatedDate;
                i.Id = n.Id;
                i.Published = n.Published;
                i.AllowComment = n.AllowComment;
                i.Title = contentLanguages.GetValue(i.Id, "Title");
                i.UrlImage = contentLanguages.GetValue(i.Id, "UrlImage");
                i.SeoUrlFriendly = contentLanguages.GetValue(i.Id, "SeoUrlFriendly");

                rows.Add(i);
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CategoryTree(string checkedOnload)
        {
            List<string> arr = new List<string>();

            if (!string.IsNullOrEmpty(checkedOnload))
            {
                checkedOnload = HttpUtility.UrlDecode(checkedOnload);
                arr = checkedOnload.ToLower().Split(',').ToList();
            }

            var data = new FeTreeNodeBuilder().GetAllCategoryForSideMenu(true, LanguageId, new List<Enums.CategoryType>()
            {
                Enums.CategoryType.News,Enums.CategoryType.NewsAndProduct
            }, false);

            foreach (var dt in data)
            {
                if (arr.Contains(dt.id.ToLower()))
                {
                    dt.state = new FeTreeNode.State() { @checked = true, selected = true, opened = true };
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }

}