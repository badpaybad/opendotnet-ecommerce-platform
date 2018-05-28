using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("UserMessageTransaction")]
    public class UserMessageTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ToUserId { get; set; }
        [StringLength(2048)]
        public string To { get; set; }
        [StringLength(2048)]
        public string ToName { get; set; }
        [StringLength(2048)]
        public string From { get; set; }
        [StringLength(2048)]
        public string Subject { get; set; }
        public string Content { get; set; }
        public short Status { get; set; }
        public short Type { get; set; }
        public string Error { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SendDate { get; set; }
    }
}
