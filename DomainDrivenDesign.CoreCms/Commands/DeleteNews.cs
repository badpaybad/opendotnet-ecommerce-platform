using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class DeleteNews:AdminBaseCommand
    {
        public DeleteNews(Guid id
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}