using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminOrderShippingController : AdminBaseController
    {
        // GET: Admin/AdminOrderShipping
        public ActionResult Index()
        {
            return View();
        }
    }
}