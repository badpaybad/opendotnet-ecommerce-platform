using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class ChangeCategoryDisplayOrder:AdminBaseCommand
    {
        public Guid Id { get; }
        public int DisplayOrder { get; }

        public ChangeCategoryDisplayOrder(Guid id, int displayOrder
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            DisplayOrder = displayOrder;
        }
    }
}