using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipAddedOneFromWithManyTo : IEvent {
        public Guid FromId { get; }
        public List<Guid> ToIds { get; }
        public string FromTableName { get; }
        public string ToTableName { get; }

        public RelationShipAddedOneFromWithManyTo(Guid fromId, List<Guid> toIds, string fromTableName, string toTableName)
        {
            FromId = fromId;
            ToIds = toIds;
            FromTableName = fromTableName;
            ToTableName = toTableName;
        }

        public long Version { get; set; }
    }
}