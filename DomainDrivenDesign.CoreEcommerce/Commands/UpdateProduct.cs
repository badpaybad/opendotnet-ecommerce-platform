using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class UpdateProduct : AdminBaseCommand
    {
        public UpdateProduct(Guid id, string productCode, bool allowComment, long price,int gram,int calorie,long quantity, string title, string shortDesciption, string description, string urlImage, Guid languageId

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
            AllowComment = allowComment;
        }

        public Guid Id { get; private set; }
        public string ProductCode { get; }
        public string Title { get; private set; }
        public string ShortDesciption { get; private set; }
        public string Description { get; private set; }
        public string UrlImage { get; }
        public Guid LanguageId { get; private set; }
        public long Price { get; private set; }
        public int Gram { get; }
        public int Calorie { get; }
        public long Quantity { get; }
        public bool AllowComment { get; private set; }
    }
}