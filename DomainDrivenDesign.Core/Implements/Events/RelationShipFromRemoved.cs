using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipFromRemoved : IEvent {
        public Guid FromId { get; }
        public string FromTableName { get; }
        public long Version { get; set; }

        public RelationShipFromRemoved(Guid fromId, string fromTableName)
        {
            FromId = fromId;
            FromTableName = fromTableName;
        }
    }
}