using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("Right")]
    public class Right
    {
        [Key]
        public Guid Id { get; set; }
      
        [StringLength(512)]
        public string Title { get; set; }
        [StringLength(4096)]
         
        public string KeyName { get; set; }
        public int Type { get; set; }
        [StringLength(512)]
        public string ReturnType { get; set; }
        [StringLength(4096)]
        public string GroupName { get; set; }
        [StringLength(4096)]
        public string Description { get; set; }
    }
}