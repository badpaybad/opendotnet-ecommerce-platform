using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsUpdated : IEvent
    {
        public Guid Id { get; }
        public bool AllowComment { get; }
        public string Title { get; }

        public NewsUpdated(Guid id, bool allowComment, string title)
        {
            Id = id;
            AllowComment = allowComment;
            Title = title;
        }

        public long Version { get; set; }
    }
}