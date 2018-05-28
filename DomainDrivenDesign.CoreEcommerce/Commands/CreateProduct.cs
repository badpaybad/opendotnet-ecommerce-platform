using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class CreateProduct : AdminBaseCommand
    {
        public CreateProduct(Guid id,string productCode, bool allowComment, long price,int gram,int calorie,long quantity, string title, string shortDesciption, string description,string urlImage, Guid languageId, Guid parentId

            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            ProductCode = productCode;
            Price = price;
            Gram = gram;
            Calorie = calorie;
            Quantity = quantity;
            Title = title;
            ShortDesciption = shortDesciption;
            Description = description;
            UrlImage = urlImage;
            LanguageId = languageId;
            ParentId = parentId;
            AllowComment = allowComment;
        }

        public Guid Id { get; private set; }
        public string ProductCode { get; }
        public long Price { get; }
        public int Gram { get; }
        public int Calorie { get; }
        public long Quantity { get; }
        public string Title { get; private set; }
        public string ShortDesciption { get; private set; }
        public string Description { get; private set; }
        public string UrlImage { get; }
        public Guid LanguageId { get; private set; }
        public Guid ParentId { get; private set; }
        public bool AllowComment { get;  }
    }

    public class AddCommentToProduct : ICommand
    {
        public AddCommentToProduct(Guid productId, string comment, string authorName, Guid userId, Guid commentParentId)
        {
            ProductId = productId;
            Comment = comment;
            AuthorName = authorName;
            UserId = userId;
            CommentParentId = commentParentId;
        }

        public Guid ProductId { get; private set; }
        public string Comment { get; set; }
        public string AuthorName { get; set; }
        public Guid UserId { get; private set; }
        public Guid CommentParentId { get; private set; }
    }
}