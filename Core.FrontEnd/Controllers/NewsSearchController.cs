using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Services;
using Core.FrontEnd.Models;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Controllers
{
    public class NewsSearchController : CmsBaseController
    {
        public ActionResult Result(string keywords)
        {
            var model = new FeSearchPage();
            model.Keywords = keywords;
            return View(model);
        }

        [HttpPost]
        public JsonResult ListResult(string keywords,
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

            if (!string.IsNullOrEmpty(keywords))
            {

                List<ContentLanguage> contentLanguages = null;

                var tempNews = NewsSearchServices.Search(keywords, LanguageId, new List<Guid>() { }, true, xskip, xtake,
                    out contentLanguages,out total);

                if (tempNews.Count == 0)
                {
                    rows.Add(new FeNews()
                    {
                        Title = "Not found any content match with keywords"
                    });
                }
                else
                {
                    foreach (var n in tempNews)
                    {
                        var i = new FeNews();
                        i.CreatedDate = n.CreatedDate;
                        i.Id = n.Id;
                        i.Published = n.Published;
                        i.Title = contentLanguages.GetValue(n.Id, "Title");
                        i.ShortDescription = contentLanguages.GetValue(n.Id, "ShortDescription");
                        i.UrlImage = contentLanguages.GetValue(n.Id, "UrlImage");
                        i.SeoUrlFriendly = contentLanguages.GetValue(n.Id, "SeoUrlFriendly");
                      
                        rows.Add(i);
                    }
                }

            }
            else
            {
                rows.Add(new FeNews() { Title = "No keywords to search" });
            }
            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LoginRequire(true)]
        public JsonResult AddComent(Guid id, string authorName, string comment, Guid? commentParentId)
        {
            var x = commentParentId ?? Guid.Empty;

            MemoryMessageBuss.PushCommand(new AddCommentToNews(id, comment, authorName, UserSessionContext.CurrentUserId(), x));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [LoginRequire(true)]
        public JsonResult ListComments(Guid newsId, int? skip, int? take, string sortField, string orderBy)
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

            using (var db = new CoreDbContext())
            {
                var queryable = db.Comments.Where(i => i.Id == newsId && i.CommentParentId == guidEmpty);
                total = queryable.LongCount();
                rows = queryable.OrderBy(i => i.CreatedDate)
                    .Skip(xskip).Take(xtake).ToList();
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}