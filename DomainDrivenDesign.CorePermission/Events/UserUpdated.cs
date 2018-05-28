using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserUpdated : IEvent
    {
        public Guid Id { get; }
        public string Phone { get; }
        public string Email { get; }

        public UserUpdated(Guid id, string phone, string email)
        {
            Id = id;
            Phone = phone;
            Email = email;
        }

        public long Version { get; set; }
    }
}