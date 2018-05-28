using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Controllers
{
    [LoginRequire(false)]
    public class OrderController : CmsBaseController
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string orderCode)
        {
            ShoppingCart order;
            using (var db=new CoreEcommerceDbContext())
            {
                order = db.ShoppingCarts.SingleOrDefault(
                    i => i.OrderCode.Equals(orderCode, StringComparison.OrdinalIgnoreCase));
            }
            return View(order);
        }
    }
}