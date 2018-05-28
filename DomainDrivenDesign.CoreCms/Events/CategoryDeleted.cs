using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class CategoryDeleted : IEvent
    {
        public Guid Id { get; }

        public CategoryDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}