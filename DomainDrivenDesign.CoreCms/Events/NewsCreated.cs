using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class NewsCreated : IEvent
    {
        public Guid Id { get; }
        public Guid ParentId { get; }
        public string Title { get; }
        public bool AllowComment { get; }
        public DateTime CreatedDate { get; }

        public NewsCreated(Guid id, Guid parentId, string title, bool allowComment, DateTime createdDate)
        {
            Id = id;
            ParentId = parentId;
            Title = title;
            AllowComment = allowComment;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }
   
}
