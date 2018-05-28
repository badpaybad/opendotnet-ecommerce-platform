using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class CategoryRootChanged : IEvent
    {
        public Guid Id { get; }
        public Guid ParentId { get; }

        public CategoryRootChanged(Guid id, Guid parentId)
        {
            Id = id;
            ParentId = parentId;
        }

        public long Version { get; set; }
    }
}