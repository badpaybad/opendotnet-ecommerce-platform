using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace DomainDrivenDesign.CoreEcommerce.Services
{
    public class ProductSearchServices
    {
        public static List<Product> Search(string keywords, Guid? languageId, List<Guid> categoryIds
            , bool? published
            , int? skip, int? take
            , out List<ContentLanguage> contentLanguages,out long total)
        {
            var xskip = skip ?? 0;
            var xtake = take ?? 10;

            var isEmptyKeywords = string.IsNullOrEmpty(keywords);

            Expression<Func<Product, bool>> productPredicate = n => true;
            if (published != null)
            {
                var xpublished = published.Value;
                productPredicate = n => n.Published == xpublished;
            }
           

            if (isEmptyKeywords)
            {
                return ResultNoKeywords(categoryIds, out contentLanguages, productPredicate
                    , xskip, xtake,out total);
            }

            var result = ResultWithKeywords(keywords, languageId, categoryIds, out contentLanguages
                , productPredicate, xskip, xtake,out total);

            return result;
        }

        private static List<Product> ResultWithKeywords(string keywords, Guid? languageId, List<Guid> categoryIds
            , out List<ContentLanguage> contentLanguages,
             Expression<Func<Product, bool>> productPredicate, int xskip, int xtake,out long total)
        {
            List<Product> result;
            var isSearchWithCategory = categoryIds.Count > 0;
            using (var db = new CoreEcommerceDbContext())
            {
                var queryIds = db.ContentLanguages.Where(
                    i => i.TableName.Equals("Product", StringComparison.OrdinalIgnoreCase)
                         && i.ColumnValue.Contains(keywords));

                if (languageId != null)
                {
                    var xlang = languageId.Value;
                    queryIds = queryIds.Where(i => i.LanguageId == xlang);
                }

                if (isSearchWithCategory)
                {
                    queryIds = queryIds.Join(db.RelationShips, p => p.Id, rs => rs.ToId, (p, rs) => new { P = p, Rs = rs })
                        .Where(m => categoryIds.Contains(m.Rs.FromId)).Select(i => i.P).Distinct();
                }

                var ids = queryIds.Select(i => i.Id).Distinct().ToList();
                total = queryIds.LongCount();
                result = db.Products
                    .Where(productPredicate)
                    .Where(i => ids.Contains(i.Id))
                    .OrderBy(i => i.CreatedDate)
                    .Skip(xskip)
                    .Take(xtake)
                    .ToList();

                var idsForContentLanguages = result.Select(i => i.Id).ToList();

                contentLanguages = db.ContentLanguages.Where(i => idsForContentLanguages.Contains(i.Id)).ToList();
            }
            return result;
        }

        private static List<Product> ResultNoKeywords(List<Guid> categoryIds, out List<ContentLanguage> contentLanguages,
            Expression<Func<Product, bool>> productPredicate, int xskip, int xtake,out long total)
        {
            var isSearchWithCategory = categoryIds.Count > 0;
            using (var db = new CoreEcommerceDbContext())
            {
                var tempNoKeywords = db.Products.AsQueryable();

                if (isSearchWithCategory)
                {
                    tempNoKeywords = tempNoKeywords.Join(db.RelationShips, p => p.Id, rs => rs.ToId,
                            (p, rs) => new { P = p, Rs = rs })
                        .Where(m => categoryIds.Contains(m.Rs.FromId)).Select(i => i.P).Distinct();
                }

                var queryable = tempNoKeywords.Where(productPredicate);
                total = queryable.LongCount();
                var resultNoKeywords = queryable
                    .OrderBy(i => i.CreatedDate)
                    .Skip(xskip)
                    .Take(xtake)
                    .ToList();

                var ids = resultNoKeywords.Select(i => i.Id).ToList();

                contentLanguages = db.ContentLanguages.Where(i => ids.Contains(i.Id)).ToList();

                return resultNoKeywords;
            }
        }
    }
}
