using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.CorePermission.Events
{

    public class UserDeleted : IEvent
    {
        public Guid Id { get; }

        public UserDeleted(Guid id)
        {
            Id = id;
        }

        public long Version { get; set; }
    }
}
