using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Events;
using System;
using System.Linq;

namespace DomainDrivenDesign.CoreCms
{
    public class DomainCategory : AggregateRoot
    {
        public override string Id { get; set; }

        public DomainCategory()
        {
        }

        void Apply(CategoryCreated e)
        {
            Id = e.Id.ToString();
        }

        bool IsExistedTitle(string title, Guid? id)
        {
            var xid = id ?? Guid.Empty;
            using (var db = new CoreDbContext())
            {
                if (db.ContentLanguages.Any(i => (id == null || i.Id != xid)
                 && i.TableName.Equals("Category", StringComparison.OrdinalIgnoreCase)
                && i.ColumnName.Equals("Title", StringComparison.OrdinalIgnoreCase)
                && i.ColumnValue.Equals(title, StringComparison.OrdinalIgnoreCase))
                )
                {
                    return true;
                }

            }
            return false;
        }

        bool IsExistFriendlyUrl(string urlsegement)
        {
            using (var db = new CoreDbContext())
            {
                if (db.ContentLanguages.Any(i => i.TableName.Equals("Category", StringComparison.OrdinalIgnoreCase)
                && i.ColumnName.Equals("SeoUrlFriendly", StringComparison.OrdinalIgnoreCase)
                && i.ColumnValue.Equals(urlsegement, StringComparison.OrdinalIgnoreCase))
                )
                {
                    return true;
                }

            }
            return false;
        }

        public DomainCategory(Guid id, bool isSinglePage, bool showInFrontEnd, string title
            , string seoKeywords, string seoDescription, string seoUrlFriendly
            , string categoryViewName, string iconUrl, string description
          , Guid languageId, Guid parentId, Enums.CategoryType type)
        {
            //if (IsExistedTitle(title, null)) throw new Exception("Title was duplicated");

            ApplyChange(new ContentLanguageUpdated(id, languageId, "Title", title, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "IconUrl", iconUrl, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "Category"));
            var seoUrl = title.ToUrlSegment();
            if (!string.IsNullOrEmpty(seoUrlFriendly))
            {
                seoUrl = seoUrlFriendly;
            }
            int seoCount = 0;
            while (IsExistFriendlyUrl(seoUrl))
            {
                seoUrl = seoUrl + "-" + seoCount;
                seoCount++;
            }
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoUrlFriendly", seoUrl, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoKeywords", seoKeywords, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoDescription", seoDescription, "Category"));
            ApplyChange(new UrlFriendlyCreated(seoUrl, "Category", id, "Category", "Index"));
            ApplyChange(new CategoryCreated(id, isSinglePage, showInFrontEnd, parentId, categoryViewName, type));

        }

        public void Update(bool isSinglePage, bool showInFrontEnd, string title
            , string seoKeywords, string seoDescription, string seoUrlFriendly
            , string categoryViewName, string iconUrl, string description
            , Guid languageId, Enums.CategoryType type)
        {
            var id = Guid.Parse(Id);

            ApplyChange(new ContentLanguageUpdated(id, languageId, "Title", title, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "IconUrl", iconUrl, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "Description", description, "Category"));
            var seoUrl = title.ToUrlSegment();
            if (!string.IsNullOrEmpty(seoUrlFriendly))
            {
                seoUrl = seoUrlFriendly;
            }
            else
            {
                if (IsExistedTitle(title, id))
                {
                    int seoCount = 0;
                    while (IsExistFriendlyUrl(seoUrl))
                    {
                        seoUrl = seoUrl + "-" + seoCount;
                        seoCount++;
                    }
                }
            }

            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoUrlFriendly", seoUrl, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoKeywords", seoKeywords, "Category"));
            ApplyChange(new ContentLanguageUpdated(id, languageId, "SeoDescription", seoDescription, "Category"));
            ApplyChange(new UrlFriendlyCreated(seoUrl, "Category", id, "Category", "Index"));

            ApplyChange(new CategoryUpdated(id, isSinglePage, showInFrontEnd, categoryViewName, type));

        }

        public void ChangeRoot(Guid parentId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new CategoryRootChanged(id, parentId));
        }

        public void Delete()
        {
            var id = Guid.Parse(Id);
            ApplyChange(new CategoryDeleted(id));
        }

        public void ChangeDisplayOrder(int displayOrder)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new CategoryChangedDisplayOrder(id, displayOrder));
        }
    }

}
