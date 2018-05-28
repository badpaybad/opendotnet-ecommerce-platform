using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class CategoryCreated : IEvent
    {
        public Guid Id { get; }
        public bool IsSinglePage { get; }
        public bool ShowInFrontEnd { get; }
        public Guid ParentId { get; }
        public string CategoryViewName { get; }
        public Enums.CategoryType Type { get; }

        public CategoryCreated(Guid id, bool isSinglePage, bool showInFrontEnd, Guid parentId, string categoryViewName, Enums.CategoryType type)
        {
            Id = id;
            IsSinglePage = isSinglePage;
            ShowInFrontEnd = showInFrontEnd;
            ParentId = parentId;
            CategoryViewName = categoryViewName;
            Type = type;
        }

        public long Version { get; set; }
    }
}
