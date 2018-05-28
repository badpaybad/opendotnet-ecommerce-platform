using System;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class UpdateUser : AdminBaseCommand
    {
        public Guid Id { get; }
        public string Phone { get; }
        public string Email { get; }

        public UpdateUser(Guid id, string phone, string email
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            Id = id;
            Phone = phone;
            Email = email;
        }
        
    }
}