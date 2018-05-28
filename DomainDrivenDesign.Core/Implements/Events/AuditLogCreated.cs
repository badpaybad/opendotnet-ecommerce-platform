using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
   public  class AuditLogCreated:IEvent
    {
        public string CommandType { get; }
        public string CommandData { get; }
        public Guid UserId { get; }
        public DateTime CreatedDate { get; }

        public AuditLogCreated(string commandType, string commandData, Guid userId, DateTime createdDate )
        {
            CommandType = commandType;
            CommandData = commandData;
            UserId = userId;
            CreatedDate = createdDate;
        }

        public long Version { get; set; }
    }
}
