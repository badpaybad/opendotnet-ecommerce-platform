using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductPriceChanged:IEvent
    {
        public Guid Id { get; }
        public long Price { get; }

        public ProductPriceChanged(Guid id, long price)
        {
            Id = id;
            Price = price;
        }
        public long Version { get; set; }
    }

    public class ProductAsComboAdded : IEvent
    {
        public Guid Id { get; }
        public List<Guid> ProductIds { get; }

        public ProductAsComboAdded(Guid id, List<Guid> productIds)
        {
            Id = id;
            ProductIds = productIds;
        }

        public long Version { get; set; }
    }
}