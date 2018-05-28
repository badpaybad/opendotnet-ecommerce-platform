using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class PublishHomePageSection : AdminBaseCommand
    {
        public Guid Id { get; }
        public bool IsPublish { get; }

        public PublishHomePageSection(Guid id, bool isPublish
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            IsPublish = isPublish;
        }
    }
}