using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class UpdateNewsForSeo : AdminBaseCommand
    {
        public Guid Id { get; }
        public string SeoKeywords { get; }
        public string SeoDescription { get; }
        public Guid LanguageId { get; }
        public string SeoUrlFriendly { get; }

        public UpdateNewsForSeo(Guid id, string seoKeywords, string seoDescription, Guid languageId, string seoUrlFriendly 
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            SeoKeywords = seoKeywords;
            SeoDescription = seoDescription;
            LanguageId = languageId;
            SeoUrlFriendly = seoUrlFriendly;
        }
    }
}