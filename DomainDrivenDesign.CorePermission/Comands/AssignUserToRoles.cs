using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class AssignUserToRoles : AdminBaseCommand
    {
        public AssignUserToRoles(Guid id, List<Guid> roles
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Roles = roles;
        }

        public Guid Id { get; }
        public List<Guid> Roles { get; set; }
    }
}