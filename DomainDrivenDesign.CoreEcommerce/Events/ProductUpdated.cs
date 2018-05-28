using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductUpdated : IEvent
    {
        public Guid Id { get; }
        public long Quantity { get; }
        public int Gram { get; }
        public int Calorie { get; }
        public string ProductCode { get; }
        public bool AllowComment { get; }

        public ProductUpdated(Guid id, long quantity,int gram, int calorie, string productCode, bool allowComment)
        {
            Id = id;
            Quantity = quantity;
            Gram = gram;
            Calorie = calorie;
            ProductCode = productCode;
            AllowComment = allowComment;
        }

        public long Version { get; set; }
    }
}