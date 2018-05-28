using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class ChangeProductToCategories : AdminBaseCommand
    {
        public Guid Id { get; }
        public List<Guid> CategoryIds { get; }

        public ChangeProductToCategories(Guid id, List<Guid> categoryIds
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            CategoryIds = categoryIds;
        }
    }
}