using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminOrderPromotionController : AdminBaseController
    {
        // GET: Admin/AdminOrderPromotion
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(Guid? id, string description, long amountToDiscount, long discountAmount, bool freeShip)
        {
            if (id == null || id == Guid.Empty)
            {
                id = Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateOrderPromotion(id.Value, LanguageId, description, amountToDiscount, discountAmount, freeShip
                    , CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateOrderPromotion(id.Value, LanguageId, description, amountToDiscount, discountAmount, freeShip
                    , CurrentUserId, DateTime.Now));
            }

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteOrderPromotion(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Active(Guid id)
        {
            MemoryMessageBuss.PushCommand(new ActiveOrderPromotion(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Inactive(Guid id)
        {
            MemoryMessageBuss.PushCommand(new InactiveOrderPromotion(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List(string keywords,
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
                sortField = nameof(DomainDrivenDesign.Core.Implements.Models.User.Username);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            List<FeOrderPromotion> rows = null;
            List<ContentLanguage> contentLangs;
            using (var db = new CoreEcommerceDbContext())
            {
                rows = db.OrderPromotions.Select(i => new FeOrderPromotion()
                {
                    Id = i.Id,
                    CreatedDate = i.CreatedDate,
                    Actived = i.Actived,
                    AmountToDiscount = i.AmountToDiscount,
                    DiscountAmount = i.DiscountAmount,
                    FreeShip = i.FreeShip
                }).OrderByDescending(i => i.CreatedDate).ToList();

                var ids = rows.Select(i => i.Id).ToList();
                contentLangs = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
                total = rows.Count;
            }

            foreach (var r in rows)
            {
                r.Description = contentLangs.GetValue(r.Id, "Description");
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}