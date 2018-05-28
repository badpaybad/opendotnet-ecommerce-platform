using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductPublished : IEvent
    {
        public Guid Id { get; }

        public ProductPublished(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }

    public class ProductBoughtByCustomer:IEvent
    {
        public Guid ProductId { get; }
        public long Quantity { get; }
        public Guid UserId { get; }
        public string CustomerEmail { get; }
        public string WebSiteUrl { get; }
        public DateTime CreatedDate { get; }

        public ProductBoughtByCustomer(Guid productId, long quantity, Guid userId, string customerEmail,string webSiteUrl, DateTime createdDate)
        {
            ProductId = productId;
            Quantity = quantity;
            UserId = userId;
            CustomerEmail = customerEmail;
            WebSiteUrl = webSiteUrl;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }
}