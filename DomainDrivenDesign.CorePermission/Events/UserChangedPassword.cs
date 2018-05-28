using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserChangedPassword : IEvent
    {
        public Guid Id { get; }
        public string Password { get; }

        public UserChangedPassword(Guid id, string password)
        {
            Id = id;
            Password = password;
        }

        public long Version { get; set; }
    }
}