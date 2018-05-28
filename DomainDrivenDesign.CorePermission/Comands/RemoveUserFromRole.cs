using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class RemoveUserFromRole : AdminBaseCommand
    {
        public Guid Id { get; }
        public Guid RoleId { get; }
        public Guid OldRoleId{ get; }

        public RemoveUserFromRole(Guid id, Guid roleId, Guid oldRoleId
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            RoleId = roleId;
            OldRoleId = oldRoleId;
        }
    }
}