using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class UpdateNews : AdminBaseCommand
    {
        public UpdateNews(Guid id, string title, string shortDesciption, string description, string urlImage, bool allowComment, Guid languageId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            ShortDesciption = shortDesciption;
            Description = description;
            UrlImage = urlImage;
            LanguageId = languageId;
            AllowComment = allowComment;
        }

        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string ShortDesciption { get; private set; }
        public string Description { get; private set; }
        public string UrlImage { get; }
        public Guid LanguageId { get; private set; }
        public bool AllowComment { get;  }
    }
}