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
    public class AdminProductPromotionController : AdminBaseController
    {
        // GET: Admin/AdminProductPromotion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Save(Guid? id, string description, long productQuantity, long discountValue)
        {
            if (id == null || id == Guid.Empty)
            {
                id = Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateProductPromotion(id.Value, description, discountValue, productQuantity
                    , LanguageId, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateProductPromotion(id.Value, description, discountValue, productQuantity
                    , LanguageId, CurrentUserId, DateTime.Now));
            }

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteProductPromotion(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPromotionsToProduct(Guid productId, List<Guid> promotionIds)
        {
            var productPromo = new List<Guid>();
            promotionIds = promotionIds.Distinct().ToList();

            using (var db = new CoreEcommerceDbContext())
            {
                productPromo = db.ProductPromotions.Join(db.RelationShips, pp => pp.Id, r => r.FromId,
                        (pp, r) => new { Pp = pp, R = r }).Where(m => m.R.ToId == productId)
                        .Select(i => i.Pp.Id).ToList();
            }

            var idsToRemove = productPromo.Where(i => !promotionIds.Contains(i)).ToList();

            MemoryMessageBuss.PushCommand(new RemovePromotionsFromProduct(idsToRemove, productId, CurrentUserId, DateTime.Now));

            MemoryMessageBuss.PushCommand(new AddPromotionsToProduct(promotionIds, productId, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = productId }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePromotionsFromProduct(Guid productId, List<Guid> promotionIds)
        {
            MemoryMessageBuss.PushCommand(new RemovePromotionsFromProduct(promotionIds, productId, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = productId }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddToProducts(Guid promotionId, List<Guid> productIds)
        //{
        //    MemoryMessageBuss.PushCommand(new AddProductsToPromotion(promotionId, productIds, CurrentUserId, DateTime.Now));

        //    return Json(new { Ok = true, Data = new { Id = promotionId }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        //}

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

            List<FeProductPromotion> rows = null;
            List<ContentLanguage> contentLangs;
            using (var db = new CoreEcommerceDbContext())
            {
                rows = db.ProductPromotions.Select(i => new FeProductPromotion()
                {
                    Id = i.Id,
                    CreatedDate = i.CreatedDate,
                    ProductQuantity = i.ProductQuantity,
                    DiscountValue = i.DiscountValue,
                    FromDate = i.FromDate,
                    ToDate = i.ToDate
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