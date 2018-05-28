using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Services;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Controllers
{
    public class NewsController : CmsBaseController
    {

        public ActionResult Detail(string urlsegment)
        {
            if (string.IsNullOrEmpty(urlsegment)) return Content("404 not found news");

            var model = new FeNews();
            using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
            {
                var temp = db.UrlFriendlys.FirstOrDefault(
                    i => i.UrlSegment.Equals(urlsegment, StringComparison.OrdinalIgnoreCase)
                    && i.TableName.Equals("News", StringComparison.OrdinalIgnoreCase));
                if (temp == null)
                {
                    return Content("Not found. 404");
                }
                var id = temp.Id;
                var news = db.News.SingleOrDefault(i => i.Id == id);

                model.Id = id;
                model.AllowComment = news.AllowComment;

                model.Title = db.ContentLanguages.GetValue(id, LanguageId, "Title");

                model.ShortDescription = db.ContentLanguages.GetValue(id, LanguageId, "ShortDescription");

                model.Description = db.ContentLanguages.GetValue(id, LanguageId, "Description");

                model.SeoKeywords = db.ContentLanguages.GetValue(id, LanguageId, "SeoKeywords");
                model.UrlImage = db.ContentLanguages.GetValue(id, LanguageId, "UrlImage");

                model.SeoDescription = db.ContentLanguages.GetValue(id, LanguageId, "SeoDescription");
            }
            return View(model);
        }

      
    }
}