using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsPublished : IEvent
    {
        public Guid Id { get; }

        public NewsPublished(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
    public class NewsUnpublished : IEvent
    {
        public Guid Id { get; }

        public NewsUnpublished(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}