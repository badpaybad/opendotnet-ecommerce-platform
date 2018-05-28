using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using Core.FrontEnd.Services;

namespace Core.FrontEnd.Controllers
{
    public class FulltextSearchController : CmsBaseController
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

            var rows = FulltextSearchServices.Search(keywords, LanguageId, null, xskip, xtake);
          
            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}