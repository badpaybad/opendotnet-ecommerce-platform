using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsAddedToCategory : IEvent
    {
        public Guid Id { get; }
        public Guid CategoryId { get; }

        public NewsAddedToCategory(Guid id, Guid categoryId)
        {
            Id = id;
            CategoryId = categoryId;
        }

        public long Version { get; set; }
    }
}