using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            var model = CacheManager.GetOrSetIfNull("HomePageSections",()=> { return BuildDataHomePage(); }, 10) ;

            return View(model);
        }

        private static FeHomePage BuildDataHomePage()
        {
            var model = new FeHomePage();
            model.Sections = new List<FeHomePage.Section>();
            List<HomePageSection> sections;
            List<ContentLanguage> contentLangs;
            List<Product> products;
            List<News> news;
            List<RelationShip> relationShip;
            using (var db = new CoreEcommerceDbContext())
            {
                sections = db.HomePageSections.Where(i => i.Published).OrderBy(i => i.DisplayOrder).ToList();
                var idsSec = sections.Select(i => i.Id).ToList();
                var idsCat = sections.Select(i => i.CategoryId).ToList();

                products = db.Products.Join(db.RelationShips, p => p.Id, rs => rs.ToId, (p, rs) => new {P = p, Rs = rs})
                    .Where(m => idsCat.Contains(m.Rs.FromId)).Select(i => i.P).ToList();
                news = db.News.Join(db.RelationShips, n => n.Id, rs => rs.ToId, (n, rs) => new {N = n, Rs = rs})
                    .Where(m => idsCat.Contains(m.Rs.FromId)).Select(i => i.N).ToList();

                var idsProduct = products.Select(i => i.Id).ToList();
                idsCat.AddRange(idsProduct);
                var idsNews = news.Select(i => i.Id).ToList();
                idsCat.AddRange(idsNews);

                contentLangs = db.ContentLanguages.Where(i => idsCat.Contains(i.Id) || idsSec.Contains(i.Id))
                    .ToList();

                relationShip = db.RelationShips.Where(i => idsProduct.Contains(i.ToId) || idsNews.Contains(i.ToId)).ToList();
            }

            foreach (var s in sections)
            {
                var section = new FeHomePage.Section();
                section.Id = s.Id;
                section.Title = contentLangs.GetValue(s.Id, "Title");
                section.ViewName = s.HomePageSectionViewName;

                section.Data = new FeCategory();
                section.Data.Id = s.CategoryId;
                section.Data.Title = contentLangs.GetValue(s.CategoryId, "Title");

                section.Data.News = new List<FeCategory.NewsItem>();
                var tempNews = news.Join(relationShip, n => n.Id, rs => rs.ToId, (n, rs) => new {N = n, Rs = rs})
                    .Where(i => i.Rs.FromId == s.CategoryId).Select(i => i.N).ToList();
                foreach (var n in tempNews)
                {
                    section.Data.News.Add(new FeCategory.NewsItem()
                    {
                        Id = n.Id,
                        Title = contentLangs.GetValue(n.Id, "Title"),
                        ShortDescription = contentLangs.GetValue(n.Id, "ShortDescription"),
                        Description = contentLangs.GetValue(n.Id, "Description"),
                        UrlImage = contentLangs.GetValue(n.Id, "UrlImage"),
                        SeoUrlFriendly = contentLangs.GetValue(n.Id, "SeoUrlFriendly")
                    });
                }

                section.Data.Products = new List<FeCategory.ProductItem>();
                var tempProducts = products.Join(relationShip, p => p.Id, rs => rs.ToId, (p, rs) => new {P = p, Rs = rs})
                    .Where(i => i.Rs.FromId == s.CategoryId).Select(i => i.P).ToList();
                foreach (var p in tempProducts)
                {
                    section.Data.Products.Add(new FeCategory.ProductItem()
                    {
                        Id = p.Id,
                        Price=p.Price,
                        ProductCode= p.ProductCode,
                        Title = contentLangs.GetValue(p.Id, "Title"),
                        ShortDescription = contentLangs.GetValue(p.Id, "ShortDescription"),
                        Description = contentLangs.GetValue(p.Id, "Description"),
                        UrlImage = contentLangs.GetValue(p.Id, "UrlImage"),
                        SeoUrlFriendly = contentLangs.GetValue(p.Id, "SeoUrlFriendly")
                    });
                }

                model.Sections.Add(section);
            }
            return model;
        }
    }
}
