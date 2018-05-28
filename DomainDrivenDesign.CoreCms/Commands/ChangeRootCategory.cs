using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class ChangeRootCategory : AdminBaseCommand
    {
        public ChangeRootCategory(Guid id, Guid parentId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            ParentId = parentId;
        }

        public Guid Id { get; private set; }
        public Guid ParentId { get; private set; }
    }
}