using System;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class UpdateCategory : AdminBaseCommand
    {
        public Guid Id { get; }
        public bool IsSinglePage { get; }
        public bool ShowInFrontEnd { get; }
        public string Title { get; }
        public string SeoKeywords { get; }
        public string SeoDescription { get; }
        public string CategoryViewName { get; }
        public string IconUrl { get; }
        public string Description { get; }
        public Guid LanguageId { get; }
        public Enums.CategoryType Type { get;  }

        public UpdateCategory(Guid id, bool isSinglePage, bool showInFrontEnd,string title
            , string seoKeywords, string seoDescription
            , string categoryViewName
            , string iconUrl, string description, Guid languageId, Enums.CategoryType type
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            IsSinglePage = isSinglePage;
            ShowInFrontEnd = showInFrontEnd;
            Title = title;
            SeoKeywords = seoKeywords;
            SeoDescription = seoDescription;
            CategoryViewName = categoryViewName;
            IconUrl = iconUrl;
            Description = description;
            LanguageId = languageId;
            Type = type;
        }
    }
}