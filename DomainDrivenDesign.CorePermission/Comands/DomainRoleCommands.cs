using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class AddRightsToRole : AdminBaseCommand
    {
        public Guid RoleId { get; }
        public List<Guid> RightIds { get; }

        public AddRightsToRole(Guid roleId, List<Guid> rightIds
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            RoleId = roleId;
            RightIds = rightIds;
        }
    }

    public class CreateRole : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Title { get; }
        public string KeyName { get; }

        public CreateRole(Guid id, string title, string keyName
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            KeyName = keyName;
        }
    }

    public class DeleteRole : AdminBaseCommand
    {
        public Guid Id { get; }

        public DeleteRole(Guid id
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
        }
    }

    public class UpdateRole : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Title { get; }
        public string KeyName { get; }

        public UpdateRole(Guid id, string title, string keyName
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Title = title;
            KeyName = keyName;
        }
    }


    public class UpdateRight : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Description { get; }

        public UpdateRight(Guid id, string description, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Description = description;
        }
    }
}