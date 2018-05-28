using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminCacheController : AdminBaseController
    {
        // GET: Admin/AdminCache
        public ActionResult Index()
        {
            var keys = CacheManager.Keys;

            return View(keys);
        }

        public JsonResult ClearAll()
        {

            CacheManager.ClearAll();
            return Json(new { Ok = true, Data = new { }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ClearKeys(List<string> keys)
        {
            CacheManager.ClearKeys(keys);
            return Json(new { Ok = true, Data = new { }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
    }
}