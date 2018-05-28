using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductUnpublished : IEvent
    {
        public Guid Id { get; }

        public ProductUnpublished(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}