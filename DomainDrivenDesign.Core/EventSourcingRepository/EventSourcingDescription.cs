using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.EventSourcingRepository
{
    [Table("EventSourcingDescription")]
    public class EventSourcingDescription
    {
        [Key]
        public Guid EsdId { get; set; }
      
        [StringLength(256)]
        public string AggregateId { get; set; }
      
        public long Version { get; set; }
       
        [StringLength(512)]
        public string AggregateType { get; set; } = string.Empty;

        [StringLength(512)]
        public string EventType { get; set; } = string.Empty;

        public string EventData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}