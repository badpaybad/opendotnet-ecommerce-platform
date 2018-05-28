using System;
using System.Collections.Generic;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipAddedManyFromWithOneTo: IEvent
    {
        public Guid ToId { get; }
        public List<Guid> FromIds { get; }
        public string FromTableName { get; }
        public string ToTableName { get; }

        public RelationShipAddedManyFromWithOneTo(List<Guid> fromIds, Guid toId,  string fromTableName, string toTableName)
        {
            FromIds = fromIds;
            ToId = toId;
            FromTableName = fromTableName;
            ToTableName = toTableName;
        }

        public long Version { get; set; }
    }
}