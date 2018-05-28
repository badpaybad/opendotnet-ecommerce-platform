using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class DeleteHomPageSection : AdminBaseCommand
    {
        public Guid Id { get; }

        public DeleteHomPageSection(Guid id
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }
}