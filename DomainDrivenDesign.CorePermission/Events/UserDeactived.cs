using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserDeactived : IEvent
    {
        public Guid Id { get; }

        public UserDeactived(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}