using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;
using Core.FrontEnd.Models;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Controllers
{
    public class CategoryController : CmsBaseController
    {
        // GET: Category
        public ActionResult Index(string urlsegment, int? page)
        {
            var model = new FeCategory();
            var xskip =  0;
            var xtake =  10;
            if (page != null)
            {
                xskip = 10 * page.Value;
                xtake = xskip + 10;
            }

            Category c = null;
            List<News> news = null;
            List<Product> products = null;
            List<ContentLanguage> contentLanguages = null;
            List<Guid> idsForContentLange = new List<Guid>();
            using (var db = new CoreEcommerceDbContext())
            {
                c = db.UrlFriendlys.Join(db.Categories, f => f.Id, i => i.Id, (f, i) => new { F = f, I = i })
                   .Where(i => i.F.UrlSegment.Equals(urlsegment, StringComparison.OrdinalIgnoreCase)
                   && i.F.TableName.Equals("Category", StringComparison.OrdinalIgnoreCase))
               .Select(i => i.I).FirstOrDefault();

                if (c == null)
                {
                    return Content("404 not found category");
                }

                if (c.IsSinglePage)
                {
                    xskip = 0;
                    xtake = 1;
                }
                idsForContentLange.Add(c.Id);

                if (c.Type == (short)Enums.CategoryType.News || c.Type == (short)Enums.CategoryType.NewsAndProduct)
                {
                    var queryable = db.News.Where(i => i.Published && i.Deleted == false).Join(db.RelationShips, n => n.Id,
                            rs => rs.ToId, (n, rs) => new { N = n, Rs = rs })
                        .Where(m => m.Rs.FromId == c.Id).Select(m => m.N)
                        .Distinct();
                    model.TotalNews = queryable.LongCount();

                    news = queryable
                        .OrderByDescending(i => i.CreatedDate)
                        .Skip(xskip).Take(xtake)
                        .ToList();
                    idsForContentLange.AddRange(news.Select(i => i.Id).ToList());
                }
                if (c.Type == (short)Enums.CategoryType.Product || c.Type == (short)Enums.CategoryType.NewsAndProduct)
                {
                    var queryable = db.Products.Where(i => i.Published && i.Deleted == false).Join(db.RelationShips, p => p.Id,
                            rs => rs.ToId, (p, rs) => new { P = p, Rs = rs })
                        .Where(m => m.Rs.FromId == c.Id).Select(m => m.P)
                        .Distinct();
                    model.TotalProduct = queryable.LongCount();

                    products = queryable
                        .OrderByDescending(i => i.CreatedDate)
                        .Skip(xskip).Take(xtake)
                        .ToList();

                    idsForContentLange.AddRange(products.Select(i => i.Id).ToList());
                }

                contentLanguages = db.ContentLanguages.Where(i => idsForContentLange.Contains(i.Id)).ToList();
            }

            model.Title = contentLanguages.GetValue(c.Id, LanguageId, "Title");
            model.SeoDescription = contentLanguages.GetValue(c.Id, LanguageId, "SeoDescription");
            model.SeoKeywords = contentLanguages.GetValue(c.Id, LanguageId, "SeoKeywords");

            model.IsSinglePage = c.IsSinglePage;
            model.Type = (Enums.CategoryType)c.Type;

            if (news != null && news.Count > 0)
            {
                model.News = new List<FeCategory.NewsItem>();
                foreach (var n in news)
                {
                    var newsItem = new FeCategory.NewsItem();
                    newsItem.Id = n.Id;
                    newsItem.CreatedDate = n.CreatedDate;
                    newsItem.Title = contentLanguages.GetValue(n.Id, LanguageId, "Title");
                    newsItem.ShortDescription = contentLanguages.GetValue(n.Id, LanguageId, "ShortDescription");
                    newsItem.UrlImage = contentLanguages.GetValue(n.Id, LanguageId, "UrlImage");
                    newsItem.SeoUrlFriendly = contentLanguages.GetValue(n.Id, LanguageId, "SeoUrlFriendly");
                    newsItem.SeoDescription = contentLanguages.GetValue(n.Id, LanguageId, "SeoDescription");
                    newsItem.SeoKeywords = contentLanguages.GetValue(n.Id, LanguageId, "SeoKeywords");
                    newsItem.Description = contentLanguages.GetValue(n.Id, LanguageId, "Description");

                    model.News.Add(newsItem);
                }
            }

            if (products != null && products.Count > 0)
            {
                model.Products = new List<FeCategory.ProductItem>();
                foreach (var p in products)
                {
                    var productItem = new FeCategory.ProductItem();

                    productItem.Id = p.Id;
                    productItem.ProductCode = p.ProductCode;
                    productItem.Price = p.Price;
                    productItem.Quantity = p.Quantity;
                    productItem.Gram = p.Gram;
                    productItem.Calorie = p.Calorie;
                    productItem.CreatedDate = p.CreatedDate;
                    productItem.Title = contentLanguages.GetValue(p.Id, "Title");
                    productItem.ShortDescription = contentLanguages.GetValue(p.Id, "ShortDescription");
                    productItem.UrlImage = contentLanguages.GetValue(p.Id, "UrlImage");
                    productItem.SeoUrlFriendly = contentLanguages.GetValue(p.Id, "SeoUrlFriendly");
                    productItem.SeoDescription = contentLanguages.GetValue(p.Id, "SeoDescription");
                    productItem.SeoKeywords = contentLanguages.GetValue(p.Id, "SeoKeywords");
                    productItem.Description = contentLanguages.GetValue(p.Id, "Description");

                    model.Products.Add(productItem);
                }
            }

            if (!string.IsNullOrEmpty(c.CategoryViewName))
            {
                return View(c.CategoryViewName, model);
            }

            if (c.IsSinglePage && c.Type == (short)Enums.CategoryType.News)
            {
                return View("~/Views/Category/CategorySinglePage.cshtml", model);
            }

            if (c.IsSinglePage && c.Type == (short)Enums.CategoryType.Product)
            {
                return View("~/Views/CategoryProduct/CategorySinglePage.cshtml", model);
            }

            return View(model);
        }
    }
}