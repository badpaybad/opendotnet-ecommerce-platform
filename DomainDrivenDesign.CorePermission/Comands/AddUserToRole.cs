using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class AddUserToRole : AdminBaseCommand
    {
        public Guid Id { get; }
        public Guid RoleId { get; }

        public AddUserToRole(Guid id, Guid roleId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            RoleId = roleId;
        }
    }
}