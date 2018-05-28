using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("ContactUsInfo")]
    public class ContactUsInfo
    {   
        [Key]
         
        public Guid Id { get; set; }
         
        public Guid ParentId { get; set; }
         
        public Guid LanguageId { get; set; }
        [StringLength(512)]
        public string FromName { get; set; }
        [StringLength(512)]
        public string FromEmail { get; set; }
        [StringLength(512)]
        public string FromPhone { get; set; }
        [StringLength(512)]
        public string ToEmail { get; set; }
        [StringLength(512)]
        public string ToPhone { get; set; }
        [StringLength(2048)]
        public string Title { get; set; }
       
        public string Body { get; set; }
        //[Column(TypeName = "DateTime2")]
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public int SentCounter { get; set; }
    }
}