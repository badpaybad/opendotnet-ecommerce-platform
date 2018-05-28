using System;
using DomainDrivenDesign.Core.Events;

namespace DomainDrivenDesign.Core.Implements.Events
{
    public class RelationShipAdded:IEvent
    {
        public Guid FromId { get; }
        public Guid ToId { get; }
        public string FromTableName { get; }
        public string ToTableName { get; }
        public int DisplayOrder { get; }
        public long Version { get; set; }

        public RelationShipAdded(Guid fromId, Guid toId, string fromTableName, string toTableName, int displayOrder)
        {
            FromId = fromId;
            ToId = toId;
            FromTableName = fromTableName;
            ToTableName = toTableName;
            DisplayOrder = displayOrder;
        }
    }
}