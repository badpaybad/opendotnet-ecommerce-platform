using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class UpdateHomePageSection : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Title { get; }
        public Guid CategoryId { get; }
        public Guid LanguageId { get; }
        public short DisplayOrder { get; }
        public string ViewName { get; }

        public UpdateHomePageSection(Guid id, string title, Guid categoryId, Guid languageId, short displayOrder, string viewName

            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            CategoryId = categoryId;
            LanguageId = languageId;
            DisplayOrder = displayOrder;
            ViewName = viewName;
        }
    }
}