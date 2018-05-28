using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        public Guid AlId { get; set; }
     
        [StringLength(512)]
        public string CommandType { get; set; }

        public string CommandData { get; set; }

        public Guid UserId { get; set; }
        ////[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}