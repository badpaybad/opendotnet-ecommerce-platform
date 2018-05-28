using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{
    public class UserLogedin : IEvent
    {
        public Guid Id { get; }
        public string TokenSession { get; }
        public DateTime TokenSessionExpiredDate { get; }

        public UserLogedin(Guid id, string tokenSession, DateTime tokenSessionExpiredDate)
        {
            Id = id;
            TokenSession = tokenSession;
            TokenSessionExpiredDate = tokenSessionExpiredDate;
        }

        public long Version { get; set; }
    }
}