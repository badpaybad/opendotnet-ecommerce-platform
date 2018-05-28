using System;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CoreCms.Events
{
    public class CategoryUpdated : IEvent
    {
        public Guid Id { get; }
        public bool IsSinglePage { get; }
        public bool ShowInFrontEnd { get; }
        public string CategoryViewName { get; }
        public Enums.CategoryType Type { get; }

        public CategoryUpdated(Guid id, bool isSinglePage, bool showInFrontEnd, string categoryViewName, Enums.CategoryType type)
        {
            Id = id;
            IsSinglePage = isSinglePage;
            ShowInFrontEnd = showInFrontEnd;
            CategoryViewName = categoryViewName;
            Type = type;
        }

        public long Version { get; set; }
    }
}