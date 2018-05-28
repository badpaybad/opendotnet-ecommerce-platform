using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsDeleted : IEvent
    {
        public Guid Id { get; }

        public NewsDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}