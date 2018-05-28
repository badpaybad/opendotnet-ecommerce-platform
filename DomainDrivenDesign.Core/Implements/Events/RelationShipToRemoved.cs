using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipToRemoved : IEvent
    {
        public Guid ToId { get; }
        public string ToTableName { get; }
        public long Version { get; set; }

        public RelationShipToRemoved(Guid toId,string toTableName)
        {
            ToId = toId;
            ToTableName = toTableName;
        }
    }
}