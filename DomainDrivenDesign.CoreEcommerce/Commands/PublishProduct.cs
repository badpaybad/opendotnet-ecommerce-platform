using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreEcommerce.Commands
{
    public class PublishProduct : AdminBaseCommand
    {
        public Guid Id { get; }
        public bool IsPublish { get; }

        public PublishProduct(Guid id, bool isPublish
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            IsPublish = isPublish;
        }
    }
}