using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreEcommerce.Events
{
    public class ProductDeleted : IEvent
    {
        public Guid Id { get; }

        public ProductDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}