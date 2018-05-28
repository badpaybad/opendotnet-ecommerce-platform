using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;

namespace DomainDrivenDesign.CoreCms.Services
{
    public class NewsSearchServices
    {
        public static List<News> Search(string keywords, Guid? languageId, List<Guid> categoryIds
            , bool? published 
            , int? skip, int? take
            , out List<ContentLanguage> contentLanguages, out long total)
        {
            var xskip = skip ?? 0;
            var xtake = take ?? 10;
          
            var isEmptyKeywords = string.IsNullOrEmpty(keywords);
          
            Expression<Func<News, bool>> newsPredicate = n => true;
            if (published!=null)
            {
                var xpublished = published.Value;
                newsPredicate = n => n.Published == xpublished;
            }
            var isSearchWithCategory = categoryIds.Count > 0;
            
            if (isEmptyKeywords)
            {
                return ResultNoKeywords(categoryIds, out contentLanguages, isSearchWithCategory
                    , newsPredicate, xskip, xtake,out total);
            }

            var result = ResultWithKeywords(keywords, languageId, categoryIds, out contentLanguages
                , isSearchWithCategory, newsPredicate, xskip, xtake,out total);

            return result;
        }

        private static List<News> ResultWithKeywords(string keywords, Guid? languageId, List<Guid> categoryIds, out List<ContentLanguage> contentLanguages,
            bool isSearchWithCategory, Expression<Func<News, bool>> newsPredicate
            , int xskip, int xtake,out long total)
        {
            List<News> result;

            using (var db = new CoreCmsDbContext())
            {
                var queryIds = db.ContentLanguages.Where(
                    i => i.TableName.Equals("News", StringComparison.OrdinalIgnoreCase)
                         && i.ColumnValue.Contains(keywords));

                if (languageId != null)
                {
                    var xlang = languageId.Value;
                    queryIds = queryIds.Where(i => i.LanguageId == xlang);
                }

                if (isSearchWithCategory)
                {
                    queryIds = queryIds.Join(db.RelationShips, n => n.Id, rs => rs.ToId, (n, rs) => new {N = n, Rs = rs})
                        .Where(m => categoryIds.Contains(m.Rs.FromId)).Select(i => i.N).Distinct();
                }

                var idsForNews = queryIds.Select(i => i.Id).Distinct().ToList();

                total = queryIds.LongCount();

                result = db.News
                    .Where(newsPredicate)
                    .Where(i => idsForNews.Contains(i.Id))
                    .OrderBy(i => i.CreatedDate)
                    .Skip(xskip)
                    .Take(xtake)
                    .ToList();

                var idsForContentLanguages = result.Select(i => i.Id).ToList();

                contentLanguages = db.ContentLanguages.Where(i => idsForContentLanguages.Contains(i.Id)).ToList();
            }
            return result;
        }

        private static List<News> ResultNoKeywords(List<Guid> categoryIds, out List<ContentLanguage> contentLanguages, bool isSearchWithCategory,
            Expression<Func<News, bool>> newsPredicate, int xskip, int xtake,out long total)
        {
            using (var db = new CoreCmsDbContext())
            {
                var tempNoKeywords = db.News.AsQueryable();

                if (isSearchWithCategory)
                {
                    tempNoKeywords = tempNoKeywords.Join(db.RelationShips, n => n.Id, rs => rs.ToId,
                            (n, rs) => new {N = n, Rs = rs})
                        .Where(m => categoryIds.Contains(m.Rs.FromId)).Select(i => i.N).Distinct();
                }

                total = tempNoKeywords.LongCount();

                var resultNoKeywords = tempNoKeywords.Where(newsPredicate)
                    .OrderBy(i => i.CreatedDate)
                    .Skip(xskip)
                    .Take(xtake)
                    .ToList();

               var newsIdWithEmptyKeywords = resultNoKeywords.Select(i => i.Id).ToList();

                contentLanguages = db.ContentLanguages.Where(i => newsIdWithEmptyKeywords.Contains(i.Id)).ToList();

                return resultNoKeywords;
            }
        }
    }
}
