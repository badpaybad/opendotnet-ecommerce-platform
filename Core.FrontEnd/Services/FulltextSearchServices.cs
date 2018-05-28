using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CoreCms.Ef;
using Microsoft.Ajax.Utilities;

namespace Core.FrontEnd.Services
{
    public static class FulltextSearchServices
    {
        public static List<FeSearchResult> Search(string keywords, Guid? languageId, List<Guid> categoryIds
            , int skip, int take)
        {
            if (string.IsNullOrEmpty(keywords)) return new List<FeSearchResult>();
            skip = skip * 3;
            take = take * 3;
            List<ContentLanguage> idsFound = null;
            using (var db = new CoreCmsDbContext())
            {
                var idsQuery = db.ContentLanguages.Where(i => i.ColumnValue.Contains(keywords));
                if (languageId != null)
                {
                    idsQuery = idsQuery.Where(i => i.LanguageId == languageId);
                }
                if (categoryIds != null && categoryIds.Count > 0)
                {
                    idsQuery = idsQuery.Join(db.RelationShips, cl => cl.Id, rs => rs.ToId,
                            (cl, rs) => new { Cl = cl, Rs = rs })
                            .Where(m => categoryIds.Contains(m.Rs.FromId))
                        .Select(m => m.Cl);
                }
                var ids = idsQuery.OrderByDescending(i=>i.CreatedDate)
                    .Skip(skip).Take(take)
                    .Select(i => i.Id).Distinct().ToList();

                idsFound = db.ContentLanguages.Where(i=>ids.Contains(i.Id))
                   .Where(i => i.ColumnName.Equals("Title", StringComparison.OrdinalIgnoreCase)
                   || i.ColumnName.Equals("SeoUrlFriendly", StringComparison.OrdinalIgnoreCase)
                   || i.ColumnName.Equals("UrlImage", StringComparison.OrdinalIgnoreCase))
                   .OrderByDescending(i => i.CreatedDate)
                   .Skip(skip).Take(take).ToList();

            }
            List<FeSearchResult> result = new List<FeSearchResult>();

            foreach (var cl in idsFound)
            {
                var r = new FeSearchResult();
                r.Id = cl.Id;
                r.Title = cl.ColumnName.Equals("Title", StringComparison.OrdinalIgnoreCase) ? cl.ColumnValue : idsFound.GetValue(r.Id,"Title");
                r.SeoUrlFriendly = cl.ColumnName.Equals("SeoUrlFriendly", StringComparison.OrdinalIgnoreCase) ? cl.ColumnValue : idsFound.GetValue(r.Id, "SeoUrlFriendly");
                r.UrlImage = cl.ColumnName.Equals("UrlImage", StringComparison.OrdinalIgnoreCase) ? cl.ColumnValue : idsFound.GetValue(r.Id, "UrlImage");
                r.TableName = cl.TableName;
                result.Add(r);
            }

            return result.DistinctBy(i=>i.SeoUrlFriendly).ToList();
        }
    }
}