using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("RelationShip")]
    public class RelationShip
    {
        [Key]
        [Column(Order = 0)]
         
        public Guid FromId { get; set; }
         
        [StringLength(128)]
        public string FromTableName { get; set; }

        [Key]
        [Column(Order = 1)]
         
        public Guid ToId { get; set; }
         
        [StringLength(128)]
        public string ToTableName { get; set; }
        public int DisplayOrder { get; set; }
    }
}