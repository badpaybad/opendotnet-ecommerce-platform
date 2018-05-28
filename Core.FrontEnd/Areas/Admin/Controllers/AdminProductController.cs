using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CoreEcommerce.Services;
using DomainDrivenDesign.CorePermission;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminProductController : AdminBaseController
    {
        public ActionResult Index(Guid? id)
        {
            var model = new ProductListAdminPage();
            if (null != id)
            {
                var xid = id.Value;
                using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
                {
                    model.CategoryId = xid;
                    model.CategoryTitle = db.ContentLanguages.GetValue(xid, LanguageId, "Title", "Category");

                }
            }

            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            var model = new ProductEditAdminPage();
            List<ContentLanguage> contentLanguages;
            using (var db = new DomainDrivenDesign.CoreEcommerce.Ef.CoreEcommerceDbContext())
            {
                var p = db.Products.SingleOrDefault(i => i.Id == id) ?? new Product();

                model.Id = id;
                model.AllowComment = p.AllowComment;
                model.ProductCode = p.ProductCode;
                model.Price = p.Price;
                model.Quantity = p.Quantity;
                model.Published = p.Published;
                model.CreatedDate = p.CreatedDate;
                model.Gram = p.Gram;
                model.Calorie = p.Calorie;

                model.ProductsInCombo = db.Products.Join(db.ProductInCombos, px => px.Id, c => c.ProductComboId,
                        (px, c) => new { P = px, C = c })
                    .Where(m => m.C.ProductId == id).Select(m => new ProductEditAdminPage.ProductInCombo
                    {
                        Id = m.P.Id,
                        Price = m.P.Price,
                        ProductCode = m.P.ProductCode,
                        Published = m.P.Published
                    }).ToList();

                var ids = new List<Guid>();
                ids.Add(id);
                ids.AddRange(model.ProductsInCombo.Select(i => i.Id).ToList());

                model.ProductCategoies = db.RelationShips.Where(i => i.ToId == id).Select(i => i.FromId).ToList();

                model.Galleries = db.PhotoGalleries.Where(i => i.Id == id).Select(i => new ProductEditAdminPage.Image()
                {
                    Id = i.PgId,
                    UrlImage = i.UrlImage
                }).ToList();

                model.Promotions = db.ProductPromotions.Join(db.RelationShips, pp => pp.Id, r => r.FromId
                    , (pp, r) => new { R = r, Pp = pp }).Where(m => m.R.ToId == id)
                    .Select(m => new FeProductPromotion()
                    {
                        Id=m.Pp.Id,
                        DiscountValue= m.Pp.DiscountValue,
                        CreatedDate= m.Pp.CreatedDate,
                        ProductQuantity= m.Pp.ProductQuantity,
                        FromDate= m.Pp.FromDate,
                        ToDate= m.Pp.ToDate
                    }).OrderByDescending(m=>m.CreatedDate).ToList();

                model.Suppliers = db.Suppliers.Join(db.RelationShips.Where(sr => sr.FromId == id), s => s.Id,
                    sr => sr.ToId
                    , (s, sr) => new {S = s, Sr = sr}).Select(i => i.S).ToList();

                ids.AddRange(model.Promotions.Select(i => i.Id).ToList());

                contentLanguages = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();

            }

            model.Title = contentLanguages.GetValue(id, "Title");
            model.ShortDescription = contentLanguages.GetValue(id, "ShortDescription");
            model.Description = contentLanguages.GetValue(id, "Description");
            model.SeoUrlFriendly = contentLanguages.GetValue(id, "SeoUrlFriendly");
            model.SeoKeywords = contentLanguages.GetValue(id, "SeoKeywords");
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

        [HttpPost, ValidateInput(false)]
        public JsonResult Save(Guid id, string productCode, bool allowComment, long price,int gram,int calorie, long quantity, string title, string shortDescription, string description, string urlImage, bool isSaveNew = false)
        {
            if (id == Guid.Empty || isSaveNew)
            {
                id = Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateProduct(id, productCode, allowComment, price,gram,calorie, quantity, title, shortDescription, description, urlImage, LanguageId, Guid.Empty, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateProduct(id, productCode, allowComment, price,gram,calorie, quantity, title, shortDescription, description, urlImage, LanguageId, CurrentUserId, DateTime.Now));
            }

            return Json(new { Ok = true, Data = new { Id = id, Title = title }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSeo(Guid id, string seoKeywords, string seoDescription, string seoUrlFriendly)
        {
            MemoryMessageBuss.PushCommand(new UpdateProductForSeo(id, seoKeywords, seoDescription, LanguageId, seoUrlFriendly, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteProduct(id, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Publish(Guid id, bool isPublish)
        {
            MemoryMessageBuss.PushCommand(new PublishProduct(id, isPublish, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveCategories(Guid id, List<Guid> categoryIds)
        {
            categoryIds = categoryIds.Where(i => i != Guid.Empty).ToList();

            MemoryMessageBuss.PushCommand(new ChangeProductToCategories(id, categoryIds, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveProductsInCombo(Guid id, List<Guid> productIds)
        {
            productIds = productIds ?? new List<Guid>();
            productIds = productIds.Where(i => i != Guid.Empty && i != id).ToList();

            MemoryMessageBuss.PushCommand(new AddProductsToCombo(id, productIds, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult List(Guid? categoryId, List<Guid> categoryIds, string keywords,
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

            List<FeProduct> rows = new List<FeProduct>();
            List<ContentLanguage> contentLanguages = null;

            categoryIds = categoryIds ?? new List<Guid>();

            if (categoryId != null && categoryId.Value != Guid.Empty)
            {
                categoryIds.Add(categoryId.Value);
            }
            var producs = ProductSearchServices.Search(keywords, LanguageId, categoryIds, null, xskip, xtake,
                out contentLanguages, out total);

            foreach (var p in producs)
            {
                var i = new FeProduct();
                i.CreatedDate = p.CreatedDate;
                i.Id = p.Id;
                i.Price = p.Price;
                i.ProductCode = p.ProductCode;
                i.Published = p.Published;
                i.AllowComment = p.AllowComment;
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

            var data = new FeTreeNodeBuilder().GetAllCategoryForSideMenu(false, LanguageId, new List<Enums.CategoryType>()
            {
                Enums.CategoryType.Product,Enums.CategoryType.NewsAndProduct
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

        [HttpPost]
        public JsonResult RemoveImages(Guid id, List<string> urlImgs)
        {
            MemoryMessageBuss.PushCommand(new RemoveImagesFromProduct(id, urlImgs, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddImages(Guid id, List<string> urlImgs)
        {
            MemoryMessageBuss.PushCommand(new AddImagesToProduct(id, urlImgs, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult AddComent(Guid id, string comment, Guid? commentParentId)
        {
            var x = commentParentId ?? Guid.Empty;
            string authorName = UserSessionContext.CurrentUsername();
            MemoryMessageBuss.PushCommand(new AddCommentToProduct(id, comment, authorName, UserSessionContext.CurrentUserId(), x));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ListComments(Guid? productId, int? skip, int? take, string sortField, string orderBy)
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
            var xnewsId = productId ?? guidEmpty;

            if (xnewsId == guidEmpty)
            {
                using (var db = new CoreDbContext())
                {
                    var queryable = db.Comments.Where(i => i.CommentParentId == guidEmpty);
                    total = queryable.LongCount();
                    rows = queryable.OrderBy(i => i.CreatedDate)
                        .Skip(xskip).Take(xtake).ToList();
                }
            }
            else
            {
                using (var db = new CoreDbContext())
                {
                    var queryable = db.Comments.Where(i => i.Id == xnewsId && i.CommentParentId == guidEmpty);
                    total = queryable.LongCount();
                    rows = queryable.OrderBy(i => i.CreatedDate)
                        .Skip(xskip).Take(xtake).ToList();
                }
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}