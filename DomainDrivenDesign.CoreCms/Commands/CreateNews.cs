using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class CreateNews : AdminBaseCommand
    {
        public CreateNews(Guid id, string title, string shortDesciption, string description, string urlImage, bool allowComment, Guid languageId, Guid parentId, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            ShortDesciption = shortDesciption;
            Description = description;
            UrlImage = urlImage;
            AllowComment = allowComment;
            LanguageId = languageId;
            ParentId = parentId;
        }

        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string ShortDesciption { get; private set; }
        public string Description { get; private set; }
        public string UrlImage { get; }
        public bool AllowComment { get; }
        public Guid LanguageId { get; private set; }
        public Guid ParentId { get; private set; }
    }
}