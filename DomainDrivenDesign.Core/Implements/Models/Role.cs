using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        [Index]
        public Guid Id { get; set; }
        [StringLength(512)]
        public string Title { get; set; }
        [StringLength(4096)]
         
        public string KeyName { get; set; }
    }
}