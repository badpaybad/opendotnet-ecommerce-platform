using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{

    [Table("User")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
      
        [StringLength(512)]
         
        public string Username { get; set; }
         
        public string Password { get; set; }
        [StringLength(512)]
        public string Email { get; set; }
        [StringLength(512)]
        public string Phone { get; set; }
        public bool Actived { get; set; }

        [StringLength(512)]
        public string TokenSession { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime TokenSessionExpiredDate { get; set; }
        public bool Deleted { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
    }
}