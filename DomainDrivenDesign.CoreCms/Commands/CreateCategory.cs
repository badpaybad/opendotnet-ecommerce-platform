using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements.Commands;
using System;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class CreateCategory : AdminBaseCommand
    {
        public CreateCategory(Guid id, bool isSinglePage, bool showInFrontEnd, string title
            , string seoKeywords, string seoDescription, string seoUrlFriendly
            , string categoryViewName, string iconUrl, string description, Guid languageId, Guid parentId, Enums.CategoryType type
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            IsSinglePage = isSinglePage;
            ShowInFrontEnd = showInFrontEnd;
            Title = title;
            SeoKeywords = seoKeywords;
            SeoDescription = seoDescription;
            SeoUrlFriendly = seoUrlFriendly;
            CategoryViewName = categoryViewName;
            IconUrl = iconUrl;
            Description = description;
            LanguageId = languageId;
            ParentId = parentId;
            Type = type;
        }

        public Guid Id { get; private set; }
        public bool IsSinglePage { get; }
        public bool ShowInFrontEnd { get; }
        public string Title { get; private set; }
        public string SeoKeywords { get; }
        public string SeoDescription { get; }
        public string SeoUrlFriendly { get; }
        public string CategoryViewName { get; }
        public string IconUrl { get; private set; }
        public string Description { get; private set; }
        public Guid LanguageId { get; private set; }
        public Guid ParentId { get; private set; }
        public Enums.CategoryType Type { get; }
    }
}