using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;

namespace Core.FrontEnd.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Detail(string urlsegment)
        {
            if (string.IsNullOrEmpty(urlsegment)) return Content("404 not found product");

            var model = new FeProduct();
            List<ContentLanguage> contentLanguages;
            Guid id = Guid.Empty;
            using (var db = new DomainDrivenDesign.CoreEcommerce.Ef.CoreEcommerceDbContext())
            {
                var temp = db.UrlFriendlys.FirstOrDefault(
                    i => i.UrlSegment.Equals(urlsegment, StringComparison.OrdinalIgnoreCase)
                         && i.TableName.Equals("Product", StringComparison.OrdinalIgnoreCase));
                if (temp == null)
                {
                    return Content("404 not found product");
                }

                 id = temp.Id;

                var product = db.Products.FirstOrDefault(i => i.Id == id);

                if (product == null)
                {
                    return Content("404 not found product");
                }

                model.Id = id;

                model.Quantity = product.Quantity;
                model.Gram = product.Gram;
                model.Calorie = product.Calorie;
                model.ProductCode = product.ProductCode;

                model.AllowComment = product.AllowComment;
                model.Price = product.Price;
                model.IsCombo = product.IsCombo;
               
                model.Galleries = db.PhotoGalleries.Where(i => i.Id == id).Select(i => i.UrlImage).ToList();

                model.ProductsInCombo = db.Products.Join(db.ProductInCombos, px => px.Id, c => c.ProductComboId,
                        (px, c) => new { P = px, C = c })
                    .Where(m => m.C.ProductId == id).Select(m => new FeProduct.ProductInCombo
                    {
                        Id = m.P.Id,
                        Price = m.P.Price,
                        ProductCode = m.P.ProductCode,
                        Published = m.P.Published
                    }).ToList();

                model.Promotions = db.ProductPromotions.Join(db.RelationShips, pp => pp.Id, r => r.FromId
                        , (pp, r) => new { R = r, Pp = pp }).Where(m => m.R.ToId == id)
                    .Select(m => new FeProductPromotion()
                    {
                        Id = m.Pp.Id,
                        DiscountValue = m.Pp.DiscountValue,
                        CreatedDate = m.Pp.CreatedDate,
                        ProductQuantity = m.Pp.ProductQuantity,
                        FromDate = m.Pp.FromDate,
                        ToDate = m.Pp.ToDate
                    }).OrderByDescending(m => m.CreatedDate).ToList();

             
                var ids = new List<Guid>();
                ids.Add(id);
                ids.AddRange(model.ProductsInCombo.Select(i => i.Id).ToList());
                ids.AddRange(model.Promotions.Select(i => i.Id).ToList());

                contentLanguages = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();
            }

            model.Title = contentLanguages.GetValue(id, "Title");
            model.ShortDescription = contentLanguages.GetValue(id, "ShortDescription");
            model.Description = contentLanguages.GetValue(id, "Description");
            model.SeoKeywords =contentLanguages.GetValue(id, "SeoKeywords");
            model.UrlImage = contentLanguages.GetValue(id, "UrlImage");
            model.SeoDescription = contentLanguages.GetValue(id, "SeoDescription");

            foreach (var pc in model.ProductsInCombo)
            {
                pc.Title = contentLanguages.GetValue(pc.Id, "Title");
            }
            foreach (var pm in model.Promotions)
            {
                pm.Description = contentLanguages.GetValue(pm.Id, "Description");
            }

            return View(model);
        }
    }
}