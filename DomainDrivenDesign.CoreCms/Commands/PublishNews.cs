using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CoreCms.Commands
{
    public class PublishNews : AdminBaseCommand
    {
        public Guid Id { get; }
        public bool IsPublish { get; }

        public PublishNews(Guid id, bool isPublish
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            IsPublish = isPublish;
        }
    }
}