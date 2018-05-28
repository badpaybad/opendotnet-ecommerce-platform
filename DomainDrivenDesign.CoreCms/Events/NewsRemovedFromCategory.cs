using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsRemovedFromCategory : IEvent
    {
        public Guid Id { get; }
        public Guid CategoryId { get; }

        public NewsRemovedFromCategory(Guid id, Guid categoryId)
        {
            Id = id;
            CategoryId = categoryId;
        }

        public long Version { get; set; }
    }
}