using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class DeleteProduct : AdminBaseCommand
    {
        public DeleteProduct(Guid id
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}