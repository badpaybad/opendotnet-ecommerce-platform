using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductCreated : IEvent
    {
        public Guid Id { get; }
        public string ProductCode { get; }
        public bool AllowComment { get; }
        public Guid ParentId { get; }
        public string Title { get; }
        public long Price { get; }
        public int Gram { get; }
        public int Calorie { get; }
        public long Quantity { get; }
        public DateTime CreatedDate { get; }

        public ProductCreated(Guid id, string productCode, bool allowComment, long price,int gram, int calorie, long quantity, Guid parentId, string title, DateTime createdDate)
        {
            Id = id;
            ProductCode = productCode;
            AllowComment = allowComment;
            ParentId = parentId;
            Title = title;
            CreatedDate = createdDate;
            Price = price;
            Gram = gram;
            Calorie = calorie;
            Quantity = quantity;
        }

        public long Version { get; set; }
    }
}
