using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipUpdated : IEvent
    {
        public RelationShipUpdated(Guid oldFromId, Guid oldToId, Guid fromId, Guid toId, string fromTableName, string toTableName, int displayOrder)
        {
            OldFromId = oldFromId;
            OldToId = oldToId;
            FromId = fromId;
            ToId = toId;
            FromTableName = fromTableName;
            ToTableName = toTableName;
            DisplayOrder = displayOrder;
        }

        public long Version { get; set; }
        public Guid OldFromId { get; }
        public Guid OldToId { get; }
        public Guid FromId { get; private set; }
        public Guid ToId { get; private set; }
        public string FromTableName { get; private set; }
        public string ToTableName { get; private set; }
        public int DisplayOrder { get; private set; }
    }
}