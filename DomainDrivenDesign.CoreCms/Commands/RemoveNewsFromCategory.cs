using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class RemoveNewsFromCategory : AdminBaseCommand
    {
        public RemoveNewsFromCategory(Guid id, Guid categoryId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            CategoryId = categoryId;
        }

        public Guid Id { get; private set; }
        public Guid CategoryId { get; private set; }
    }
}