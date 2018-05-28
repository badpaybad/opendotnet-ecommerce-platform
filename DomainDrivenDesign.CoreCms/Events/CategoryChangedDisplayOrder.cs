using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class CategoryChangedDisplayOrder : IEvent
    {
        public Guid Id { get; }
        public int DisplayOrder { get; }

        public CategoryChangedDisplayOrder(Guid id, int displayOrder)
        {
            Id = id;
            DisplayOrder = displayOrder;
        }

        public long Version { get; set; }
    }
}